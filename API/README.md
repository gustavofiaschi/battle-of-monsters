# Customer manager

Battle of Monsters API

## Table of Contents

- [Setup](#setup)
- [Test](#usage)

## Setup

1. Use dotnet to run the migrations:
    dotnet ef migrations add Init --project API
​
2. Run the database
    dotnet ef database update --project API
​
4. Install code analyzer.
    dotnet tool install -g dotnet-format
​
5.  Run the app:
    dotnet run --project API

## Test

1. If you don't have an IDE with integrated coverage tools, follow these steps to produce a coverage report: Install the report generator.
    dotnet tool install -g dotnet-reportgenerator-globaltool
​
2. Run the tests. This command generates a resulting coverage.cobertura.xml file as output.
    dotnet test --collect:"XPlat Code Coverage"
​
3. Generate the report; use the provided options based on the coverage.cobertura.xml file from the previous test run.
reportgenerator -reports:"Path\To\TestProject\TestResults\{guid}\coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html






