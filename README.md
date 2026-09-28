<h1 style="text-align:center">Habit Logger</h1>

<p>
    Habbit Logger - Console application where you’ll log occurrences of a habit.
</p>

<h2>Requirements</h2>
<ul>
    <li>This habit can't be tracked by time (ex. hours of sleep), only by quantity (ex. number of water glasses a day)</li>
    <li>Users need to be able to input the date of the occurrence of the habit</li>
    <li>The application should store and retrieve data from a real database</li>
    <li>When the application starts, it should create a sqlite database, if one isn’t present.</li>
    <li>It should also create a table in the database, where the habit will be logged.</li>
    <li>The users should be able to insert, delete, update and view their logged habit</li>
    <li>You should handle all possible errors so that the application never crashes.</li>
</ul>

<h2>Challenges</h2>
<ul>
    <li>You can only interact with the database using ADO.NET. You can’t use mappers such as Entity Framework or Dapper.</li>
    <li>If you haven't, try using parameterized queries to make your application more secure</li>
    <li>Let the users create their own habits to track. That will require that you let them choose the unit of measurement of each habit.</li>
    <li>Seed Data into the database automatically when the database gets created for the first time, generating a few habits and inserting a hundred records with randomly generated values.</li>
</ul>

<h2>Features</h2>
The application uses a SQLite to store and read information

Basic CRUD operations

<h2>Areas to Improve<h2>
Deepening my understanding of raw SQL. I am currently taking an introductory SQL course to build on my foundational skills.

Unit tests. Skiped it for now due lack of time.


