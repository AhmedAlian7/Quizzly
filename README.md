<div align="center">
  <img src="images/quizzly-header.png" alt="Quizzly Logo" width="480" />
  <h1>Quizzly - A Quickly Online Quiz Management System</h1>
  <p>Quizzly is an online quiz management system built with ASP.NET Core that enables instructors to create, manage, and grade quizzes while providing students with an intuitive interface to take quizzes and view their results.</p>
  <p>🌐 <strong>Live Demo</strong>: <a href="http://quizzly.runasp.net">http://quizzly.runasp.net</a></p>
</div>

## 🎯 Overview

Quizzly is designed to streamline the quiz creation and management process for educational institutions. It supports multiple question types, automated and manual grading, real-time analytics, and seamless integration with external services.

## 🚀 Features

### For Instructors

#### Quiz Management
- **Create & Edit Quizzes**: Build comprehensive quizzes with multiple question types
- **Question Types Support**:
  - Multiple Choice Questions (MCQ)
  - True/False Questions
  - Short Answer Questions
  - Essay Questions
- **Quiz Configuration**:
  - Set time limits and availability windows
  - Configure shuffle options for questions and choices
  - Set passing scores and attempt limits
  - Control visibility of correct answers and scores
- **Access Control**: Generate unique access tokens for quiz distribution
- **Publish/Draft**: Control quiz availability to students

#### Grading System
- **Automated Grading**: AI-powered grading for objective questions
- **Manual Grading**: Instructor review and grading for subjective questions
- **Flexible Scoring**: Per-question point allocation
- **Grading Analytics**: Track grading patterns and student performance

#### Analytics & Reporting
- **Student Performance Analytics**: Comprehensive performance tracking
- **Question-Level Analytics**: Identify challenging questions
- **Score Distribution**: Visual representation of student performance
- **Time Analytics**: Average completion time tracking
- **Common Mistakes Analysis**: Identify patterns in incorrect answers

#### Student Management
- **Student Enrollment**: Manage student access to quizzes
- **Performance Tracking**: Monitor individual student progress
- **Grade History**: Complete grading history and feedback

#### Category Management
- **Quiz Categories**: Organize quizzes by subject or topic
- **Category-Based Analytics**: Performance insights by category

### For Students

#### Quiz Taking Experience
- **Easy Access**: Join quizzes using access tokens
- **Responsive Interface**: Mobile-friendly quiz interface
- **Question Navigation**: Easy navigation between questions
- **Time Management**: Real-time timer and progress tracking
- **Auto-Save**: Automatic saving of answers to prevent data loss

#### Results & Feedback
- **Immediate Results**: Instant feedback for auto-graded questions
- **Detailed Feedback**: Comprehensive explanations and correct answers
- **Grade History**: Access to all previous quiz attempts
- **Performance Insights**: Track improvement over time

### System Features

#### Authentication & Authorization
- **Role-Based Access Control**: Instructor and Student roles
- **External Authentication**: Google OAuth integration
- **Secure Login**: ASP.NET Identity integration
- **Account Management**: User registration and profile management

#### AI Integration
- **AI-Powered Grading**: Automated grading using Groq AI API
- **Intelligent Feedback**: AI-generated feedback for student answers
- **Consistent Scoring**: Standardized grading across subjective questions

#### Email Notifications
- **Grading Notifications**: Email alerts when manual grading is completed
- **SMTP Integration**: Configurable email service

#### File Management
- **Image Upload**: Support for question images using Cloudinary
- **File Storage**: Secure cloud-based file storage

## 🛠️ Technology Stack

### Backend Technologies
- **.NET 9.0**: Latest .NET framework for high performance
- **ASP.NET Core MVC**: Web application framework
- **Entity Framework Core 9.0**: Object-Relational Mapping (ORM)
- **SQL Server**: Primary database for data persistence

### AI & External Services
- **Groq AI API**: AI-powered grading and feedback generation
- **Cloudinary**: Cloud-based image and file storage
- **SMTP/MailKit**: Email service integration

### Authentication
- **Google OAuth 2.0**: External authentication provider
- **ASP.NET Identity**: User management and authentication
- **Role-Based Security**: Multi-level access control

### Frontend Technologies
- **ASP.NET Core MVC Views**: Server-side rendering
- **Bootstrap**: Responsive UI framework
- **JavaScript**: Interactive client-side functionality
- **CSS3**: Modern styling and responsive design

### Development Tools
- **Entity Framework Migrations**: Database version control
- **Dependency Injection**: Built-in IoC container
- **Configuration Management**: JSON-based configuration

## 📊 Database Architecture

![Database Diagram](docs/database-diagram-v02.png)

## 🏗️ Project Structure

```
Quizzly/
├── Quizzly.Web/                 # Web application layer
│   ├── Areas/                   # MVC Areas for different user types
│   │   ├── Authentication/      # Login/Registration functionality
│   │   ├── Instructor/          # Instructor-specific features
│   │   └── Student/             # Student-specific features
│   ├── Controllers/             # Main application controllers
│   ├── Views/                   # Shared views and layouts
│   └── wwwroot/                 # Static files (CSS, JS, images)
├── Quizzly.Business/            # Business logic layer
│   ├── Services/                # Service implementations
│   │   ├── Implementions/       # Concrete service implementations
│   │   └── Interfaces/          # Service contracts
│   ├── ViewModels/              # Data transfer objects
│   └── Configuration/           # Application configuration
├── Quizzly.DataAccess/          # Data access layer
│   ├── Entities/                # Domain entities
│   ├── Repositories/            # Repository pattern implementation
│   ├── Data/                    # DbContext and configurations
│   └── Migrations/              # Entity Framework migrations
└── docs/                        # Documentation and diagrams
```

## 📱 Usage Guide

### For Instructors

1. **Register/Login**: Create an instructor account or login
2. **Create Categories**: Organize quizzes by subject or topic
3. **Create Quizzes**: Build quizzes with various question types
4. **Configure Settings**: Set time limits, scoring, and access options
5. **Publish Quizzes**: Generate access tokens and share with students
6. **Monitor Progress**: View analytics and grade manual questions
7. **Manage Students**: Track student performance and access

### For Students

1. **Register/Login**: Create a student account or login
2. **Join Quiz**: Use access token provided by instructor
3. **Take Quiz**: Answer questions within time limit
4. **View Results**: See scores and feedback immediately or after manual grading
5. **Track Progress**: View history of all quiz attempts

## 🔒 Security Features

- **Role-Based Access Control**: Secure access to features based on user roles
- **External Authentication**: Google OAuth integration for secure login
- **Data Validation**: Server-side validation for all inputs
- **Anti-Forgery Tokens**: CSRF protection on forms
- **Secure Configuration**: Environment-based configuration management
- **Audit Trail**: Created/Updated timestamps for all entities


<div align="center">


**Quizzly** Making online education quizzez simple, efficient, and intelligent ✨

**Built with ❤️ by the Quizzly team**

<img src="images/quizzly-logo.png" alt="Quizzly Logo" width="60" />

</div>
