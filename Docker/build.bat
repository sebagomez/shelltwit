copy ..\bin app\
docker build -t sebagomez/twit .

REM docker run -it --name twit sebagomez/twit

REM docker commit twit mytwit

REM docker run --rm mytwit -q Irma