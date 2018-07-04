$(document).ready(function () {

    function solveAndDraw(d,n) {
        var points = solver(d, n);
        var n = points.length - 1;

        var di = 1; //jump some points when drawing, look for patterns
        var color = "#000000";
        var i = di;
        var length = 10;

        var draw = _ => {

            drawMove(points[i - di], points[i], length, color);

            i += di;
            if (i <= n) {
                setTimeout(draw, 20);
            }
            else if (di*4 < n) {

                //A: increment i one by one:
                //di++; 

                //B: increment i by multiples of n;
                //do {
                //    di++;
                //} while (n % di != 0);

                //C: powers of two seem to have a pattern (also, choose n to be a power of two)
                //di *= 2;

                //D: powers of 4 have a pattern: it follows the same pattern but each step has a larger length and starting direction varies predictably.
                di *= 4;
                i = di;
                let myColors = ["#000000","#FF0000", "#00FF00", "#0000FF", "#F0000F", "#0F0F00"];
                color = myColors[(myColors.findIndex(value => value === color) + 1) % myColors.length];
                console.log(`New di: ${di} [color: ${color}]`);
                setTimeout(draw, 20);
            }
        }

        draw();

    }

    function drawMove(fromPoint, toPoint, length, color = "#000000") {
        fromPoint = fromPoint.map(value => value * length);
        toPoint = toPoint.map(value => value * length);
        var c = document.getElementById("myCanvas");
        var ctx = c.getContext("2d");
        ctx.strokeStyle = color;
        ctx.beginPath();
        ctx.moveTo(fromPoint[0] + 400, 400 - fromPoint[1]);
        ctx.lineTo((toPoint[0]) + 400, 400 - (toPoint[1]));
        ctx.stroke();
    }


    function solver(maxD, numMoves) {

        let currentPoint = [0, 0];
        let points = [currentPoint];

        let DIRECTIONS = [[0, 1], [1, 0], [0, -1], [-1, 0]];
        let d = 4 * 1000000; //start pointing up, clock wise; r -> mod(d++); | l -> mod(d--); 
        let k = 0; //number of moves
        let p = [0, 0]; //[x,y];
        let s = [['F', 0], ['a', 0]];

        while (k < numMoves) {

            let el = s.shift();

            if (el[0] === 'F') {
                p[0] += DIRECTIONS[d % 4][0];
                p[1] += DIRECTIONS[d % 4][1];
                //console.log(`k: ${k} -> d: ${d%4}`);
                k++;
                points.push([p[0], p[1]]);
            }
            else if (el[0] === 'L') {
                d = d === 0 ? 3 : d - 1;
            }
            else if (el[0] === 'R') {
                d = d === 3 ? 0 : d + 1;
            } else if (el[0] === 'a') {
                if (el[1] < maxD) {
                    s.unshift(["a", el[1] + 1], ["R", el[1] + 1], ["b", el[1] + 1], ["F", el[1] + 1], ["R", el[1] + 1]);
                }
            } else if (el[0] === 'b') {
                if (el[1] < maxD) {
                    s.unshift(["L", el[1] + 1], ["F", el[1] + 1], ["a", el[1] + 1], ["L", el[1] + 1], ["b", el[1] + 1]);
                }
            } else {
                alert("ERROR!");
                return;
            }
        }
        console.log(`Solver points count: ${points.length}`);
        console.log(`Solver last Point: ${points[points.length-1]}`);

        return points;
    }

    $("#draw").on("click", function () {
        console.log($(this));
        let d = $("#d").val();
        let n = $("#numSteps").val();
        console.log(`${d} ${n}`);
        solveAndDraw(d,n);
    });

});

