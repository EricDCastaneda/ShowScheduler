# ShowScheduler 
ShowScheduler is a web app that lets users schedule a show at a specific venue and add bands to each show with individually assigned time slots. Click the following link to see the app: https://showschedulerapp.azurewebsites.net/  

## How to use ShowScheduler
Register an account and then click Shows>Add a Show. After you enter all the details and click Add, you will be taken to that Show’s Info page. Then you can click Add a Band and enter the Band’s details along with the Start and End time for their set. You can add multiple bands to each show if their set times don’t overlap. Each show and band can be edited. You can see all shows by clicking on Shows or you can see all bands from all shows by clicking on Bands. 

## User Stories
- A user can register an account and login/logout.
- A user can add/remove/edit a show once registered and logged in.
- A user can add/remove/edit a band once registered and logged in.

## Features
- Shows & Bands
  - Shows & Bands are validated so no empty shows/bands are allowed. 
  - When a show/band is added, it’s stored in a SQL database.
  - All shows/bands can be viewed anonymously but only registered users can add/remove/edit entries.
  - The Shows page displays all shows in descending order based on their date.
  - The Bands page displays all bands in ascending order based on their name.

