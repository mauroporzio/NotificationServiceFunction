# 📧 Rate-Limited Notification Service

## Overview

This project is a **.NET 8.0 Azure Function** that implements a **rate-limited email notification system** to prevent over-sending to recipients. It ensures each recipient receives notifications within acceptable limits defined by notification type (e.g., status updates, news, marketing).

Notifications are processed, validated, and routed based on their compliance with configured rate limits. Messages that exceed limits or encounter errors are redirected to appropriate Azure Storage Queues.

[Challenge guidelines](https://github.com/mauroporzio/NotificationServiceFunction/blob/master/Docs/Challange.pdf)

---

## 🔧 Technologies Used

- **.NET 8.0 (C#)**
- **Azure Functions**
- **Azure Storage Queues**
- **Azure Table Storage**
- **Azure Storage Explorer** (for local emulation and testing)

---

## 🚦 Rate Limit Examples

Each recipient is rate-limited based on the type of notification:

- `Status`: max **2 per minute**
- `News`: max **1 per day**
- `Marketing`: max **3 per hour**

> These rules are configurable and stored in a JSON file inside Azure Blob Storage.

---

## 📂 Architecture

![NotificationService_Challange](https://github.com/user-attachments/assets/dc76648e-944c-48ce-8766-1fec22e093cf)

### Queues

- **notificationsqueue**: Incoming notification messages.
- **rejectednotificationsqueue**: Messages rejected due to rate-limit violations.
- **notificationspoisonqueue**: Messages that failed due to unhandled or unexpected exceptions.

### Table

- **NotificationsEvent**: Stores successfully validated messages, ready to be sent.

### Azure Function

- Triggers on `notificationsqueue`.
- Applies validation and routing logic.
- Writes results to the appropriate queue or table.

---

## 📦 Project Setup (Local Windows Dev)

### 1. Clone the Repository

```bash
git clone https://github.com/mauroporzio/NotificationServiceFunction.git
```

### 2. Install Prerequisites

- [Visual Studio 2022+](https://visualstudio.microsoft.com) with .NET 8 SDK
- [Azure Storage Explorer](https://azure.microsoft.com/es-es/products/storage/storage-explorer)

### 3. Run Azure Function

- Open solution in Visual Studio.
- Build and run `NotificationServiceFunction`.

### 4. Configure Blob Storage

In Azure Storage Explorer:

1. Add a new **Blob Container** named `notifications-function-configs`.
2. Upload the config file located at:  
   `Business/ConfigurationFiles/notificationRateLimitsConfig.json`  
   *(from the cloned repo)*

Note: This JSON file contains example configurations for rate limit validations. Feel free to experiment with other values, but keep in mind that the notificationType must exist in NotificationTypesEnum.cs.

### 5. Final Structure

Your local Azure Storage should now include:

- `notificationsqueue`
- `rejectednotificationsqueue`
- `notificationspoisonqueue`
- `notifications-function-configs` blob with the config file
- `NotificationsEvent` table

---

## 🧪 Testing the Flow (With Emulator)

1. Run the Azure Function.
2. In **Azure Storage Explorer**, navigate to the `notificationsqueue`.
3. Add a new queue message with a JSON payload:

```json
{
  "recipient": "user@example.com",
  "notificationType": "Status",
  "content": "Your status has been updated!"
}
```
Note: This JSON file contains example configurations for rate limit validations. Feel free to experiment with other values, but keep in mind that the "notificationType" must exist in NotificationTypesEnum.cs. Out of the box, "notificationType" supports the values "Status", "News", and "Marketing" (additional values can be added as needed in NotificationTypesEnum.cs).

### Possible Outcomes

1. ✅ Inserted into `NotificationsEvent` table (passed validation).
2. ⚠️ Routed to `rejectednotificationsqueue` (rate limit exceeded).
3. ❌ Routed to `notificationspoisonqueue` (unexpected error).

---

## 🧠 Notes

- The actual sending of notifications is handled by a **hypothetical Notification Sender API** (not included).
- This project focuses solely on **validation and routing logic** based on rate limits.

---

## 📎 Resources

- [Project Repository](https://github.com/mauroporzio/NotificationServiceFunction)
- [Visual Studio](https://visualstudio.microsoft.com)
- [Azure Storage Explorer](https://azure.microsoft.com/es-es/products/storage/storage-explorer)
