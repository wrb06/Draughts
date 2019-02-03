
str1 = input("BoardSave\n")

char_count = 0;
print();
for i in range(64):
    if (i % 2 + i // 8) % 2 == 1:
        if str1[char_count] =='b':
            print("b.PlacePiece(new Piece(false, " + str(i%8) + "," + str(i//8) + "));")
        if str1[char_count] == 'B':
            print("b.PlacePiece(new KingPiece(false, " + str(i%8) + "," + str(i//8) + "));")
        if str1[char_count] == 'w':
            print("b.PlacePiece(new Piece(true, " + str(i%8) + "," + str(i//8) + "));")
        if str1[char_count] == 'W':
            print("b.PlacePiece(new KingPiece(true, " + str(i%8) + "," + str(i//8) + "));")
        char_count+=1

