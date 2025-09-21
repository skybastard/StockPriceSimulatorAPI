Visual Studio was used to create this project.<br>
To start, clone the repo using VS<br>
It will load folders tree, open the folder StockPriceSimulatorAPI and double click StockPriceSimulatorAPI.sln<br>
Build the solution<br>
To run the tests open Test Explorer from Visual Studio and run selected tests<br><br>
![Tests](<Readme docs/VS test explorer.png>)<br><br>
If no errors, then right click the solution and select properties, from there go to startup projects, select all three shown in the picture. Order is not important, clients wait until API is running<br><br>
![Startup projects](<Readme docs/startup projects.png>)<br><br>
Run the solution.<br>
Three windows should show up, the API, Signal IR and TCP Client<br><br>
![Three windows](<Readme docs/two clients and api.png>)<br><br>
I'm using OpenAPI, it has Swagger so it opens up the api in browser to try out GET data<br><br>
![Swagger home](<Readme docs/swagger homepage.png>)<br><br>
From there press Try it out and excecute, it should show prices and data from api<br><br>
![Swagger data](<Readme docs/swagger try out.png>)<br><br>
Second one needs you to add one name such as AAPL to get specific stock<br><br>
![Swagger data](<Readme docs/swagger add symbol and excecute.png>)<br><br>
