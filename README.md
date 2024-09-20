## PostgreSQL Database

```
CREATE TABLE users (
    id SERIAL PRIMARY KEY,        
    user_id BIGINT NOT NULL,  
    name VARCHAR(100) NOT NULL,             
    phone VARCHAR(15) NOT NULL,             
    role VARCHAR(10) CHECK (role IN ('admin', 'user')) NOT NULL, 
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);


CREATE TABLE subjects (
    id SERIAL PRIMARY KEY,                 
    name VARCHAR(100) NOT NULL,             
    max_score INT NOT NULL CHECK (max_score > 0) 
);

CREATE TABLE questions (
    id SERIAL PRIMARY KEY,                  
    subject_id INT REFERENCES subjects(id) ON DELETE CASCADE, 
    question_text TEXT NOT NULL,            
    correct_answer_id INT                 
);

CREATE TABLE answers (
    id SERIAL PRIMARY KEY,                  
    question_id INT REFERENCES questions(id) ON DELETE CASCADE, 
    answer_text TEXT NOT NULL,              
    is_correct BOOLEAN DEFAULT FALSE     
);

CREATE TABLE user_tests (
    id SERIAL PRIMARY KEY,                 
    user_id INT REFERENCES users(id) ON DELETE CASCADE, 
    subject_id INT REFERENCES subjects(id) ON DELETE CASCADE, 
    test_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP, 
    total_score INT DEFAULT 0   
);

CREATE TABLE user_answers (
    id SERIAL PRIMARY KEY,                  
    user_test_id INT REFERENCES user_tests(id) ON DELETE CASCADE, 
    question_id INT REFERENCES questions(id) ON DELETE CASCADE, 
    answer_id INT REFERENCES answers(id) ON DELETE CASCADE, 
    is_correct BOOLEAN DEFAULT FALSE        
);

CREATE TABLE results (
    id SERIAL PRIMARY KEY,                  
    user_test_id INT REFERENCES user_tests(id) ON DELETE CASCADE, 
    file_type VARCHAR(5) CHECK (file_type IN ('pdf', 'excel')) NOT NULL, 
    file_path VARCHAR(255) NOT NULL         
);
```
## Folder structure

```
/QuizBot
│
├─── /Controllers # Processing bot commands
│ ├──── AdminController.cs # Admin Commands
│ ├─── UserController.cs # User commands
│ ├──── CommandController.cs # General Commands
│
├─── /Handlers # Handle messages and inline buttons
│ ├──── UpdateHandler.Message.cs # Text message processing logic
│ ├──── UpdateHandler.Callback.cs # Logic for processing inline buttons
│ ├──── UpdateHandler.cs # Logic for 
│
├─── /Services # Services for working with data
│ ├─── UserService.cs # Work with users (CRUD)
│ ├─── AdminService.cs # Administrative actions (CRUD)
│ ├─── QuizService.cs # Work with tests (CRUD)
│ ├─── FileService.cs # Export results to PDF/Excel
│
├─── /Repositories # Work with database via Npgsql
│ ├─── UserRepository.cs # Logic of working with user table
│ ├─── QuizRepository.cs # Logic of working with tests and questions
│
├─── /Models # Data Models
│ ├─── User.cs # Model for User
│ ├──── Subject.cs # Model for subject
│ ├─── Question.cs # Model for a question
│ ├─── Answer.cs # Model for an answer
│
├──── /Keyboards # Inline keyboards
│ ├─── MainMenuKeyboard.cs # Main Menu (inline keys)
│ ├──── AdminKeyboard.cs # Keyboard for administrator
│ ├──── UserQuizKeyboard.cs # Keyboard for test selection
│
├─── /Utilities # Auxiliary utilities
│ ├─── PdfGenerator.cs # PDF generation
│ ├─── ExcelExporter.cs # Export to Excel
│
├─── /Configurations # Configuration files
│ ├─── BotConfiguration.cs # Bot token configuration
│
└─── Program.cs # Entry point
```