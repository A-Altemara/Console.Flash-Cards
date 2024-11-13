# Flash Cards Console App
Small Console App to study flash cards.

## Description
This application allows users to create and study Stacks of Flashcards. Each stack contains a set of flashcards, and users can track their study sessions. The project consists of several tables and features that manage flashcards, stacks, and study sessions

## Features
- Users can create Stacks of Flashcards.
- Flashcards are grouped under stacks, and each stack has a unique name.
- Deleting a stack automatically deletes all its associated flashcards and study sessions.
- Study Sessions are recorded when users study a stack, storing the date and score of each session.
- Reports Study Session Scores and number of attempts.

## Requirements
- Stacks must have unique names.
- Each Flashcard belongs to a stack and must be deleted if the stack is deleted.
- Flashcards must be renumbered from 1 when displayed, without any gaps.
- DTOs are used to display flashcards to users without showing stack IDs.
- Study sessions must be stored in the database and linked to stacks. Study sessions cannot be updated or deleted once created.

## Dependancies
SQL Server
Entity Framework
Docker

### Installation and Setup
- Clone the repository.
- build the docker image using the following command:
```bash docker build -t flashcards-app . ```
- run docker container using the following command:
```bash docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=P@ssword1" -p 1433:1433 --name flashcards-db -d mcr.microsoft.com/mssql/server:2019-latest ```
- Start the application using the following command:
```bash dotnet run ```
- access the application on the browser using the following URL:
```bash http://localhost:5000 ```

## MIT License
- Copyright (c) 2024 Airiel Altemara

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

## Contributing
Contributions to this project are welcome. If you find any issues or have suggestions for improvements, please open an issue or submit a pull request.