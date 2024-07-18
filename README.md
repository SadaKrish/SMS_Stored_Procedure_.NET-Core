# Student Management System
###OverView
The Student Management System is a web application built with ASP.NET Core. The project developed on the three-tier architecture. It provides functionalities for managing students, teachers, subjects, allocated teachers for subjects, and allocated subjects for students. The main concept of this project is, here stored procedure used to retrieve data from database. All the logics has been handled on the stored procedure.This project includes key features designed to enhance user-friendliness.
###Features
*Autocomplete Search: The application provides autocomplete suggestions in dropdowns while searching by specific categories, making it easier to find the desired information.
*Validation: Comprehensive validation to prevent duplication of records and ensure data integrity.
*Download Options: Data can be downloaded as PDF or Excel sheets for easy sharing and analysis.
*NLog Messages: NLog is used to log user activities. The log messages are stored in a database table and displayed in the UI. Features include:
***Generating daily reports of messages based on log level.
***Displaying log levels in a pie chart.
***Generating reports based on filtered data, which can be downloaded as PDF files.
***Date validations to ensure that the end date is not earlier than the start date when filtering data
