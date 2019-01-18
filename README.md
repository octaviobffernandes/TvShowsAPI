# TvShowsAPI


## How to:
### Run the web application
- Go to the Src folder of the repository
- Make sure appsettings is correctly configured with a valid mongodb connection string
- Execute dotnet build
- Execute dotnet run
- on the console output, you should see the startup debug messages, followed by something like: dbugNow listening on: http://localhost:65439 . This is your api url

### Get show information from our internal API
- Run a GET request against [api url]/Shows
- Or alternatively go to [api url]/swagger and fire the /Shows GET operation

### Import data from External API (TV Maze)
- Run a POST request against [api url]/Import
- Or alternatively go to [api url]/swagger and fire the /Import POST operation
PS: It is possible to queue up several times the import operation. But they will run in sequential mode, i.e., each one will wait for the previous one to finish.


### Manage status of import process
- Go to [api url]/Hangfire - this will go to the Hangfire dashboard
- Go to jobs tab
- Here you can check which jobs are queued, running, canceled, etc.
- You can stop, restart and fire new jobs from the hangfire dashboard too


### Future improvements
- Move long-running jobs to Azure Webjobs or AWS lambda instead of Hangfire
- Write proper API documentation to be available also on Swagger
- Unit tests / Integration tests
- Authentication/Authorization
- Switch to using Serilog as logging library instead of .net builtin library
