# Developer Test - Azure Functions

## Description
This test is for the Software Developer position at Provoke Solutions, Bellevue. Below you will find a list of requirements. It will be up to you to meet the requirements to the best of your ability. Please fork this repository. **Once completed, you should submit your results as a pull request back to this repository.**

## What you're building
You will be building an application from the ground up. The application is meant to help your users understand weather patterns using the [OpenWeatherMap API](https://openweathermap.org/). It doesn't need to be fancy, but it should meet the basic requirements below.

## Requirements
Overview: The backend application should be built using Azure Functions. A Timer Trigger every 5 minutes should retrieve weather data for Seattle and store it in Cosmos Db (CosmosDb has an emulator for local storage. [Use this for local development](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator)). There is an Azure Function template within Visual Studio that should get you started.

1) The backend needs to fetch data from the weather API and be resilient against the weather API going down or being overloaded (pretend that the weather api is very fragile and often goes down*).
1) Once the data is fetched, map this data to an easier-to-understand object and store it in Cosmos Db.
1) **Bonus:** If you have some extra time, create an Http trigger that allows the user to query this data by time. 

## Submitting your work
Please submit your work as a pull request into this repository and **include a short write-up of how you met each of the above requirements** when you are done.

Thanks and good luck!


*Note: We may run your application against an API wrapper for the openweathermap API that fails 60% of the time.*
