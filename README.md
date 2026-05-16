# eVoting-System-SWP316D-TUT-
Computer Science final year project under SWP316D module

# 🗳️ eVoting System

## 📖 Project Description
The eVoting System is a web-based application designed to modernize the electoral process by allowing citizens to register, verify their identity, and vote online.

The system improves efficiency, reduces human error, and enhances accessibility compared to traditional voting methods.

---

## 🎯 Objectives
- Digitize the voting process  
- Ensure secure and fair elections  
- Prevent duplicate and fraudulent voting  
- Improve voter accessibility  
- Provide efficient election management  

---

## 👥 Users (Actors)

### Voter
- Register using ID number  
- Upload proof of address (if required)  
- View registration status  
- Cast vote during elections  
- View voting history  

### Admin
- Review voter registrations  
- Approve or reject applications  
- Provide feedback  

### Super Admin
- Manage Admin accounts  
- Manage elections (start/end dates)  
- Approve political parties  

---

## ⚙️ Technologies Used
- Backend: C# ASP.NET Core Web API (.NET 8)  
- Frontend: HTML, CSS, JavaScript  
- Database: MySQL  
- Hosting: IIS / AWS  
- Authentication: JWT  

---

## 🧠 System Features

### Voter Registration
- Uses South African ID number  
- Automatically retrieves user details  
- Stores data securely  

### Voting Registration
- National voting → Automatically approved  
- Provincial voting → Requires proof of address + admin approval  

### Voting Process
- Voting only available during election period  
- One vote per election type:
  - National  
  - Provincial  
  - Regional  

### Admin Features
- View pending registrations  
- Approve or reject voters  

### Election Management
- Set election start and end dates  
- Enable/disable voting automatically  

---

## 🔐 Security Features
- JWT Authentication  
- Role-based access control  
- Prevention of duplicate voting  
- Secure file uploads  

---

## 🗳️ Voting Rules
- If Provincial is NOT selected:
  - User votes National only  
  - Automatically approved  

- If Provincial is selected:
  - User votes:
    - National  
    - Provincial  
    - Regional  
  - Requires admin approval  

---

## 🗃️ Database Tables
- Voters  
- Admins  
- SuperAdmins  
- VotingRegistrations  
- PoliticalParties  
- Elections  
- Votes  

---

## 🚀 How to Run the Project

### Backend
1. Open in Visual Studio 2022  
2. Configure database connection  
3. Run the API  

### Frontend
1. Open HTML files in browser  
2. Connect to API  

---

## 📅 Project Status
- Core system completed  
- Voting system functional  
- Admin & Super Admin modules working  

---

## 🔮 Future Improvements
- Mobile application  
- Fingerprint authentication  
- AI fraud detection  
- Blockchain integration  

---

## 👨‍💻 Authors
- Thobisani Siyethembha Mabaso  
- (Add team members)

---

## 📄 License
This project is for academic purposes.
