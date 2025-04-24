BIN = bin

OUT = $(BIN)/stasis.exe

SRC = src/*.c

LIB = -Lthirdparty/raylib/lib -lraylib -lgdi32 -lwinmm thirdparty/flecs/flecs.c -lWs2_32
INCLUDE = -Ithirdparty/raylib/include -Ithirdparty/flecs

$(OUT): $(SRC)
	gcc -o $(OUT) $(SRC) $(LIB) $(INCLUDE)

run: $(OUT)
	./$(OUT)
