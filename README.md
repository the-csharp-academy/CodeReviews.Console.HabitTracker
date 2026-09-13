# Habit tracker

First of all, I want to say that the project was quite interesting for me, yet also complex, since I always find it difficult to actually start writing code and figure out the design, and so on. Watching the tutorial video gave me a clear sense of direction regarding what to use, where, and why.

# SQL course
After watching a video where someone was using SQL commands, I realized I needed to take some SQL courses on the site; I hadn't understood the commands—even though they are quite easy to read—or the logic behind constructing queries, so I immediately started the training. It paid off: I was able to create two tables—one for storing habits and another for the habit entries themselves—and, thanks to the course, I successfully established a one-to-many relationship between them.

# But another problem also arose
It’s one thing to create two tables and set up a relationship between them, but it’s another to structure the logic so that, after selecting a habit, I can create a record for it—since I need a way to pass data between them indicating which table was chosen, and so on. The problem wasn't forgetting to create a foreign key, but rather figuring out how to write the logic for it; though I think I managed—well, almost. In the end, I got something working; even if the logic is pretty full of holes, I did manage to write it and link everything up.

# LINQ and tests
Regarding LINQ and testing: I haven't learned how to write unit tests yet, so I turned to AI for help. Also, when downloading the testing packages, they somehow ended up in the main project instead of the test project—though I managed to fix that later. As for LINQ, I plan to figure it out by working through the course on the website; that’s the approach I’m taking for now.

# Tech Stack & Prerequisites
- **Language**: C# / .NET 8
- **Database**: SQLite
- **IDE** Visual Studio 
