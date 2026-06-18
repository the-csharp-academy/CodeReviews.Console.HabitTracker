# Console Habit Tracker

A console-based CRUD application to track drinking water, written in C# and powered by SQLite.

## How It Works

The application connects to a local SQLite database using ADO.NET. Upon the very first execution, it automatically creates the drinking_water table.
The user can then add, view, update and delete records via the console.

## Challenges Completed

1. **Parameterized Queries**: All database inputs go through explicit query parameters to prevent SQL injection.

### What Was Easy?

I followed the video tutorial, which was simple enough to follow. I think I understood everything that was shown.

### What I Learned

I got a better grasp of manual parameterized queries to make interacting with the database more secure.
