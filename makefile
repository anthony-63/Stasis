BIN = bin

OUT = $(BIN)/stasis.exe

SRC = src/*.c

$(OUT): $(SRC)
	gcc -o $(OUT) $(SRC)

run: $(OUT)
	./$(OUT)
