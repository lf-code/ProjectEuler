$(document).ready(function () {

    //Problem 321;

    function Place(v,i){
        this.v = v; //0->empty, 1-> red, 2-> blue;
    }

    var board = [];

    var selectedIndex = -1;

    var n = 4;

    var c = $("#board").html();



    //resets board to its initial state
    var resetBoard = function () {


        board = [];
        for (var i = 0; i < n; i++) {
            board.push(new Place(1))
        }

        board.push(new Place(0))

        for (i = 0; i < n; i++) {
            board.push(new Place(2))
        }

        updateBoardView();
    }

    //DOM

    var updateBoardView = function () {
        $("#board").empty();
        for (var i = 0; i<board.length; i++) {
            if (board[i].v > 0) {
                var x = c.replace("green", board[i].v === 1 ? "red" : "blue");
                x = x.replace('myindex="x"','myindex="'+i+'"')
                $("#board").append(x)
            } else {
                $("#board").append('<div class="place" myindex="'+i+'"></div>')
            }
        }
    }


    //EVENTS

    $("#n").on("change", function (target) {
        //n = args.target.value;
        n = $(this).val();
        resetBoard();
    });


    //SOLVER:

    var getInitialState = function (n) {
        let a = Array(n).fill("r");
        a.push("e");
        return a.concat(Array(n).fill("b")).join("");
    }

    var getAllStates = function (n) {

        let initialState = getInitialState(n);
        console.log(initialState);

        var N = 100000;
        //Naive approach: generate N random states, and save unique states;
        //hopefully all possible states for a small n will be generated at least once:

        var set = new Set();

        set.add(initialState);
        var s = initialState;
        for (var j = 0; j < N; j++) {
            //use last generated state for increased randomness
            s = s.split("").sort((a, b) => Math.random() - 0.5).join("");
            set.add(s);
        }

        return Array.from(set);
    }

    var getTransitions = function (startState, allStates) {

        //there is only one empty space, any trasition involves moving
        //a neighbor piece of the empty place into it.
        var transitions = [];
        var i = startState.indexOf("e");
        //i-1 left neighbor (slide)
        if (i - 1 > -1) 
            transitions.push(allStates.indexOf(auxSwap(startState, i, i - 1)))

        //i+1 right neigbor (slide)
        if (i + 1 < startState.length)
            transitions.push(allStates.indexOf(auxSwap(startState,i,i+1)))
       
        //i-2 second to left neighbor (jump)
        if (i - 2 > -1 && (startState.charAt(i - 1) !== startState.charAt(i-2)))
            transitions.push(allStates.indexOf(auxSwap(startState, i, i - 2)))

        //i+2 second to right neighbor (jump)
        if (i + 2 < startState.length && (startState.charAt(i + 1) !== startState.charAt(i + 2)))
            transitions.push(allStates.indexOf(auxSwap(startState, i, i + 2)))

        return transitions;
    }

    //aux method for getTransitions
    var auxSwap = function (state, a, b) {
        var aux = state.split('');
        var x = aux[a];
        aux[a] = aux[b];
        aux[b] = x;
        return aux.join("");
    }

    const INF = 100000000;

    var Dijkstra = function (graph, source, target) {

        var dist = []; //distance to source of optimal yet;
        for (var i = 0; i < graph.length; i++)
            dist[i] = INF;

        var prev = []; //prev node of optimal yet;
        for ( i = 0; i < graph.length; i++)
            prev[i] = undefined;

        var unvisited = []; //unvisited nodes;
        for (i = 0; i < graph.length; i++)
            unvisited.push(i);

        dist[0] = 0;
        while (unvisited.length > 0) {
            var u = undefined;
            var minDist = INF + 1;
            var aux = -1;
            for ( i = 0; i < unvisited.length; i++) {
                if (dist[unvisited[i]] < minDist) {
                    u = unvisited[i];
                    minDist = dist[u];
                    aux = i;
                }
            }
            unvisited.splice(aux, 1);

            for ( i = 0; i < graph[u].length; i++) {
                var v = graph[u][i];
                var alt = dist[u] + 1; //length of all transitions == 1;
                if (alt < dist[v]) {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        var path = [];
        var h = target;
        while (prev[h] !== undefined) {
            path.unshift(h);
            h = prev[h];
        }
        path.unshift(h);
        console.log(path);
        return path;
    }

    var solve = function (n) {

        //get all possible states:
        var allStates = getAllStates(n);

        //construct graph from all possible trasitions for each state
        var graph = [];
        for (var i = 0; i < allStates.length; i++) {
            graph.push(getTransitions(allStates[i], allStates));
        }

        //get string of solved state and find it allStates:
        let solvedState = getInitialState(n).split("").reverse().join("");
        var indexSolved = allStates.indexOf(solvedState);

        console.log(`solvedState: ${solvedState} index:${indexSolved} total:${allStates.length}`);
        console.log(allStates[indexSolved]);

        //solve from Dijkstra
        var path = Dijkstra(graph, 0, indexSolved);

        var solutionStates = path.map((value) => allStates[value]);

        console.log(solutionStates);

        return solutionStates;
    }

    //Solution ANIMATION:

    var animateSolution = function (solutionStates) {

        var i = 0;
        var x = () => {
            console.log(`Showing solution state: ${i}/${solutionStates.length}`);
            showState(solutionStates[i]);
            i++;
            if (i < solutionStates.length)
                setTimeout(x, 2000);
        }

        x();

    }

    var showState = function (state) {
        console.log(`show state: ${state}`);
        $("#board").empty();
        for (var i = 0; i < state.length; i++) {
            if (state.charAt(i) !== "e") {
                var x = c.replace("green", state.charAt(i) === "r" ? "red" : "blue");
                x = x.replace('myindex="x"', 'myindex="' + i + '"')
                $("#board").append(x)
            } else {
                $("#board").append('<div class="place" myindex="' + i + '"></div>')
            }
        }
    }

    var printSolutionMoves = function (solutionState) {

        for (var i = 1; i < solutionState.length; i++) {
            //the piece that moved was in the new empty place:
            let fromIndex = solutionState[i].indexOf("e");
            let toIndex = solutionState[i-1].indexOf("e");
            let pieceColor = solutionState[i].split("")[toIndex];
            let typeOfMove = Math.abs(fromIndex - toIndex);
            console.log(`[move: ${i}] "${pieceColor}" moved from ${fromIndex} to ${toIndex} type: ${typeOfMove}`);
        }
    }

    var mySolverFromPattern = function (n) {

        //PATTERN: 

        var stateAndCount = { state: getInitialState(n), count: 0 };
        var colors = ["r", "b"];

        var i = 0;       
        var k = 1;

        //move pieces of one color k times, change color, move k+1 times, and so on,  until k==4
        while (k <= n)
            state = mySolverFromPatternAux(colors[i++ % 2], k++, stateAndCount);

        //change color, do it one more time for k==n;
        state = mySolverFromPatternAux(colors[i++ % 2], k, stateAndCount);

        //change color, start with k==n, move pieces of one color k times , change color, move k-1 times, and so on,  until k==1
        while (k >= 1)
            state = mySolverFromPatternAux(colors[i++ % 2], k--, stateAndCount);

        //returns number of moves to solve n-sized board
        return stateAndCount.count;
    }

    var mySolverFromPatternAux = function (color, k, stateAndCount) {

        var stateArray = stateAndCount.state.split("");
        var swap = (i, j) => {
            let x = stateArray[i];
            stateArray[i] = stateArray[j];
            stateArray[j] = x;
            stateAndCount.count++; //each swap is a move
        }

        //Move K pieces of a given color, starting in the direction below:

        //direction -> end-to-start for red, start-to-end for blue:
        var direction = color === "r" ?  -1 : 1;

        //start at the end of array for red, at the beginning for blue
        var index = color === "r" ? stateArray.length - 1 : 0;

        var t = 0;
        while (index >= 0 && index <= stateArray.length - 1 && t < k) {
            if (stateArray[index] === color) {
                if (stateArray[index + (direction*-1)] === "e") { //SLIDE
                    swap(index, index + (direction * -1));
                    t++;
                }
                else if (stateArray[index + (direction * -2)] === "e") { //OR JUMP
                    swap(index, index + (direction * -2));
                    t++;
                }
            }
            index += direction;
        }

        stateAndCount.state = stateArray.join("");
    }

    var solverCount = function (n) {

        //return m(n), it comes from the pattern identified in mySolverFromPattern
        //for n = 4 => 1+2+3+4 +4+ 4+3+2+1
        //given that the 1+2+3+4 is equal to Tringle number(4) => m(n) = 2*T(n)+n
        // from the definition of triangle number T(n) => (n*n+1)/2, thus m(n) = n^2 +2n;

        return n * n + 2*n;
    }

    $("#solveBtn").on("click", function () {


        var n = Number($("#n").val());
        var m = solverCount(n);
        console.log(m);


        //let solutionStates = solve(n);
        //let numberOfMoves = solutionStates.length - 1; //number of transitions
        //console.log(`M(${n}) = ${numberOfMoves}`);

        //animateSolution(solutionStates);
        //printSolutionMoves(solutionStates)
    });

    resetBoard();



});

