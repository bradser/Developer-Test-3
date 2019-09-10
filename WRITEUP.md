Requirements:

1) The backend needs to fetch data from the weather API and be resilient against the weather API going down or being overloaded (pretend that the weather api is very fragile and often goes down*).

I used the Polly library (http://www.thepollyproject.org/) to add progressive backoff to the HTTP requests. Before moving to production I'd do due-diligance to get more information about the fragility of the API, so I could tweak the heuristic if needed. And also talk to the customer to see if there is other mitigation that should be put in place.

2) Once the data is fetched, map this data to an easier-to-understand object and store it in Cosmos Db.

I created a class WeatherPeristJson. I reorganized the HTTP API response JSON as follows:
- Organized all internal properties into an Internal object.
- Organized location information into a Location object.
- Organized time-related information into a DateTime object.
- Alphabetized the class (although I know that JSON isn't speced to have an order).
- Added longer names.
- Ensured all properties with units have trailing unit characters.
- Miscellanous reorganization.

3) **Bonus:** If you have some extra time, create an Http trigger that allows the user to query this data by time. 

I added the WeatherQueryHttpTrigger. The URL takes a query string parameter named 'hours', which lets you select how many hours into the past the query should collect data, from present time. If the query string parameter is missing, the default is 12 hours.
