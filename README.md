# MvcAgeApp
quick and basic asp.net core mvc demo

The project relies on a localdb being created and named 'MvcAgeApp'. The script (DbTableCreate.sql) to create the 1 table 
which the app uses is located in the root folder of the project. Create the db locally and run the script before running the app.
the app assumes windows authentication to the db. This can be changed in the connection string if required.

App demonstrates a basic mvc controller with a couple of roots, some validation, basic styling, and basic sorting and filtering.

There is also an xUnit tests project which runs some basic tests on the controller.

