# 🔑 Keylogger & Client-Server File Transfer

Este proyecto es una combinación de un **Keylogger** y un **Cliente-Servidor** para transferencia de archivos a través de **TCP** con `ngrok`. 🖥️💾

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
📦 Keylogger-ClientServer
├── 📂 Client
│   ├── 🔹 ClientSocket.cs  # Lógica de conexión y envío de archivos
│   └── 🔹 Program.cs       # Punto de entrada del cliente
├── 📂 Keylogger
│   ├── 🔹 Program.cs       # Captura de teclas y almacenamiento en Log.txt
└── 📂 Server
    ├── 🔹 ServerSocket.cs  # Lógica de recepción de archivos
    └── 🔹 Program.cs       # Punto de entrada del servidor
```

---

## 🛠 **Uso**
1. **Inicia el servidor** ejecutando el programa en la carpeta `Server/`
2. **Ejecuta el cliente** en la máquina destino
3. **El keylogger registrará las pulsaciones** y enviará archivos si es necesario.

---

## ⚠️ **Notas de Seguridad** ⚠️
🔒 Este proyecto está diseñado **exclusivamente para fines educativos** y **pruebas de seguridad**.  
❌ **No lo uses para actividades maliciosas o ilegales.**

---

## 📌 **Créditos**
Desarrollado con ❤️ por [Tu Nombre].

---

## 📜 **Licencia**
Este proyecto se distribuye bajo la licencia MIT. 📄

