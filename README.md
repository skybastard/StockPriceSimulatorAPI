This project has a stock price simulator, REST api updates stocks for 5 stocks every five seconds and keeps history for each stock for the last 10 prices.<br><br>
Here are the basic features:<br>
Stock Simulator (core)<br>
REST API (Swagger, endpoints)<br>
SignalR Hub → SignalR Console Client<br>
TCP Server → TCP Console Client<br>
Plugin Loader (CSV / JSON formatters)<br>
Tests (unit + integration)<br>
Serilog logging<br><br>

Basic flow<br><br>
![Tests](<Readme docs/sequence_flow.png>)<br><br>




Visual Studio was used to create this project.<br>
To start, clone the repo using VS<br>
First time after cloning it might only load the load folders tree, if so, open the folder StockPriceSimulatorAPI and double click StockPriceSimulatorAPI.sln<br>
Build the solution<br>
To run the tests open Test Explorer from Visual Studio and run selected tests<br><br>
![Tests](<Readme docs/VS test explorer.png>)<br><br>
If no errors, then right click the solution and select properties, from there go to startup projects, select all three shown in the picture. Order is not important, clients wait until API is running<br><br>
![Startup projects](<Readme docs/startup projects.png>)<br><br>
Run the solution.<br>
Three windows should show up, the API, SignalR and TCP Client<br><br>
![Three windows](<Readme docs/two clients and api.png>)<br><br>
I'm using OpenAPI, it has Swagger so it opens up the api in browser to try out GET data<br><br>
![Swagger home](<Readme docs/swagger homepage.png>)<br><br>
From there press Try it out and excecute, it should show prices and data from api<br><br>
![Swagger data](<Readme docs/swagger try out.png>)<br><br>
Second one needs you to add one name such as AAPL to get specific stock<br><br>
![Swagger data](<Readme docs/swagger add symbol and excecute.png>)<br><br>
