Recommendit 2.0 is here. 


I've started this project as one of my first projects that I've ever coded in .NET.

Looking at it few years later, well, let's just say that it's always good to be embarrassed at our earlier code. :)

I've added some improvements since. Upgraded on the architecture, created indexes for faster lookup of shows and changed Show Vectors query to fetch from a non-sql MongoDB instead, for a blazingly faster lookup.

I've also trained a different model this time, BERT, which is better optimized to fetch sentences.


I've made a copy of the "THETVDB" Database, a directory of over 200 000 thousand shows which allows you to lookup shows ( Much faster this time, due to the extended non-clustering index knowledge and query performance tuning).

After selecting 3 shows, the user presses generate and waits for the cosine similarity calculation of the vectors (array of doubles) of the shows you've just chosen and all other shows in the database. The array of doubles is where the BERT Machine Learning module helped.
It transferred the description of every show, to an array of doubles, with around 200 elements (each) for more precise similarity calculation.


Recommendit is made with ASP.NET, SQL Server, MongoDB, the BERT ML Model (I've used Python for this model).

