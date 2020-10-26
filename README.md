# Draughts

Repo for my sixth form computer science project. I wanted to write a program to play chess, but was advised against it because I would spent too much time making the game and not enough on the algorithm which would score me the marks. We comprimised and settled on draughts.

The AI player uses an algorithm called minimax, which according to [Wikipedia](https://en.wikipedia.org/wiki/Minimax), "Minimax (sometimes MinMax, MM[1] or saddle point[2]) is a decision rule used in artificial intelligence, decision theory, game theory, statistics, and philosophy for minimizing the possible loss for a worst case (maximum loss) scenario. When dealing with gains, it is referred to as "maximin"—to maximize the minimum gain". In simpler terms, it thinks ahead trying to find the best moves that I can make by steering the game towards a favourable position for me.  

Technically I am using a version called [Negamax]( https://en.wikipedia.org/wiki/Negamax) which, simply, works because the best move that I can do to improve my "score" also lowers your "score" by the same amount. As well as this you can enable [Alpha-Beta pruning]( https://en.wikipedia.org/wiki/Alpha%E2%80%93beta_pruning), which seeks to prune the search to only include favourable results and improve search time.

As the search for moves is computationally expensive, multithreading is used to separate the GUI and keep it responsive while the search is done elsewhere, not affecting the responsiveness. There is a progress bar which tells the user how close the search is to being completed.

![Image](https://github.com/wrb06/Draughts/blob/master/ExampleGame.png)
