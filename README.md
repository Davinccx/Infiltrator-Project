# 🔑 Infiltrator Proyect 1.0
Este proyecto es un troyano con múltiples funcionalidades, incluyendo transferencia de archivos y captura de información a través de TCP con ngrok. 🖥️💾

---

## 📌 **Características**
✅ Captura las pulsaciones de teclado y las almacena en `Log.txt` 📜
✅ Envío y recepción de archivos entre el cliente y el servidor 📡
✅ Uso de **TCP** para la comunicación entre procesos 🌐
✅ Integración con **ngrok** para exponer el servidor a Internet 🔗
✅ Manejo de errores y reconexión automática 🚦

---

## 🚀 **Instalación y Configuración**

### 🔹 **Requisitos Previos**
- .NET 6+ 🛠️
- `ngrok` instalado ([Descargar aquí](https://ngrok.com/download)) 📥
- Firewall configurado para permitir `ngrok` 📶

### 🔹 **Configuración del Servidor**
1. Ejecuta `ngrok` para tunelizar el puerto TCP:
   ```sh
   ngrok tcp 443
   ```
2. Obtén la dirección proporcionada por `ngrok` (ejemplo: `tcp://0.tcp.eu.ngrok.io:15821`).
3. Copia la dirección y actualiza `serverAddr` y `serverPort` en el código del **cliente**.
4. Inicia el servidor.

### 🔹 **Configuración del Cliente**
1. Asegúrate de actualizar la dirección `serverAddr` con la IP y puerto de `ngrok`.
2. Compila y ejecuta el cliente.

---

## 📂 **Estructura del Proyecto**
```plaintext
📦 Infiltrator Proyect
├── 📂 Client
│   ├── 📂 Commands
│   ├── 📂 Conexion
│   ├── 📂 Crypto
│   ├── 📂 Native
│   ├── 📂 Stealers
│   ├── 📂 Util
│   │   ├── 🔹 Functions.cs
│   │   ├── 🔹 Log.cs
│   │   ├── 🔹 Screenshot.cs
│   │   ├── 🔹 SystemInfo.cs
│   ├── 🔹 ClienteRAT.cs
│   ├── 🔹 Keylogger.cs
├── 📂 Server
│   ├── 📂 Conexion
│   ├── 📂 Crypto
│   ├── 📂 GUI
│   ├── 🔹 Config.cs
│   ├── 🔹 Logger.cs
│   ├── 🔹 ServidorRAT.cs
```

---

## 🛠 **Uso**
1. **Inicia el servidor** ejecutando el programa en la carpeta `Server/`
2. **Ejecuta el cliente** en la máquina destino


---

## ⚠️ **Notas de Seguridad** ⚠️
🔒 Este proyecto está diseñado **exclusivamente para fines educativos** y **pruebas de seguridad**.  
❌ **No lo uses para actividades maliciosas o ilegales.**

---

## 📌 **Créditos**
Desarrollado con ❤️ por David Fernández Sanz.

---

## 📜 **Licencia**
Este proyecto se distribuye bajo la licencia MIT. 📄

