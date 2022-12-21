# Voice-Server-VoIP

This project is the voice server part of a messenger, which was implemented as a university project in the second semester in the advanced programming course. This project is implemented using C# language and WPF.

What this project does is that it receives a message in the form of voice from a client and sends it to the destination client that has its IP and port. The interesting thing is that if the destination client was offline, the server will send the message again when the destination client is online.

> Socket programming has been used to implement these concepts.


## How to use:

- When you run the project in Visual Studio (Both Client & Server), These 2 console screens will be displayed and you can send message to Server:


![image](https://github.com/Ali-Roodi79/Socket-programming-asynchronous-client-and-server/blob/main/assets/MainConsolePage.png)

---

- For example, we send the sentence **"Hello server"** to the server, and the server must send us exactly the same message.:


![image](https://github.com/Ali-Roodi79/Socket-programming-asynchronous-client-and-server/blob/main/assets/Send%26ReceiveMessage.png)

---

- The process of sending and receiving messages is all done on the basis of **socket programming** and completely **asynchronously**.

- The messages are sent & receive using the IP address of the local host, **127.0.0.1** and port **9999**.
