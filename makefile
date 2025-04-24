BIN = bin

OUT = $(BIN)/stasis.exe

SRC = src/*.c

LIB = -Lthirdparty/SDL3/lib -lSDL3dll
INCLUDE = -Ithirdparty/SDL3/include

$(OUT): $(SRC)
	gcc -o $(OUT) $(SRC) $(LIB) $(INCLUDE)

run: $(OUT)
	./$(OUT)
