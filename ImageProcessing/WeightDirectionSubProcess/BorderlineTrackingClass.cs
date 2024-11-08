/// <summary>
/// 輪郭線追跡クラス
/// </summary>
public static class BorderlineTrackingClass {
  class Position {
    public int x;
    public int y;
        
    public Position(int x, int y) {
        this.x = x;
        this.y = y;
    }
    
    public string getString() {
        return $"({this.x}, {this.y})";
    }
}
        
    private static int[,] DirectionPoint = 
    {
        { -1, -1, 0, 1, 1, 1, 0, -1 },
        { 0, 1, 1, 1, 0, -1, -1, -1 },
    };
    
    private static byte[,] image =  
    {
        {0,0,0,0,0,0,0},
        {0,0,1,1,0,0,0},
        {0,1,1,1,1,1,0},
        {0,0,1,1,1,1,0},
        {0,0,1,1,1,1,0},
        {0,1,1,1,1,1,0},
        {0,0,0,0,0,0,0},
    };
    
    private static int[,] Directions16 =  
    {
        {14,13,12,11,10},
        {15,14,12,10, 9},
        { 0, 0,99, 8, 8},
        { 1, 2, 4, 6, 7},
        { 2, 3, 4, 5, 6},
    };
    
    private static int GetPrevToNextDirection(Position currentPos, Position prevPos)
    {
        int dx = prevPos.x - currentPos.x;
        int dy = prevPos.y - currentPos.y;
        
        for (int i = 0; i < DirectionPoint.GetLength(1); i++) {
            if (dx == DirectionPoint[0, i] && dy == DirectionPoint[1, i]) {
                return i;
            }
        }
        return 9;
    }
    private static int GetNextDirection(byte[,] image, Position currentPos, int startDirection)
    {
        for (int i = 0; i < DirectionPoint.GetLength(1); i++) {
            int nextDirection = (i + startDirection) % 8;
            int nextX = currentPos.x + DirectionPoint[0, nextDirection];
            int nextY = currentPos.y + DirectionPoint[1, nextDirection];
            
            // 配列範囲外チェック
            if (nextX < 0 || nextX >= image.GetLength(1) || nextY < 0 || nextY >= image.GetLength(0)) 
                continue;
            
            if (image[nextY, nextX] == 0) continue;
            
            return (i + startDirection) % 8;
        }
        //Console.WriteLine("該当なし");
        return 0;
    }
    private static Position GetNextPosition(Position currentPos, int direction)
    {
            int nextX = currentPos.x + DirectionPoint[0, direction];
            int nextY = currentPos.y + DirectionPoint[1, direction];
            
            return new Position(nextX, nextY);
    }
    
    private static bool IsSamePosition(Position a, Position b)
    {
        if (a.x == b.x && a.y == b.y) {
            return true;
        }
        return false;
    }
    
    private static void OutlineTracking(byte[,] image, int[,] track, Position startPos, int startDirection)
    {
        Position currentPos = startPos;
        image[currentPos.y, currentPos.x] = 7;
        int nextDirection = GetNextDirection(image, currentPos, startDirection);
        Position prevPos = currentPos;
        Position nextPos = GetNextPosition(currentPos, nextDirection);
        //Console.WriteLine($"prevPos={prevPos.getString()} currentPos={currentPos.getString()} nextPos={nextPos.getString()}");
        
        
        do{
            prevPos = currentPos;
            currentPos = nextPos;
            
            startDirection = (GetPrevToNextDirection(currentPos, prevPos)+1)%8;
            //Console.WriteLine($"next search startDir = {startDirection} current = {currentPos.getString()}");
            nextDirection = GetNextDirection(image, currentPos, startDirection);
            nextPos = GetNextPosition(currentPos, nextDirection);
            int Direction = Directions16[nextPos.y - prevPos.y + 2,nextPos.x - prevPos.x + 2];
            image[currentPos.y, currentPos.x] = 7;
            track[currentPos.y, currentPos.x] = Direction;
            //Console.WriteLine($"prevPos={prevPos.getString()} currentPos={currentPos.getString()} nextDir={nextDirection} nextPos={nextPos.getString()} D:{Direction}");
            //if (currentPos.x == 99 && currentPos.y == 99) break;
        }while(!IsSamePosition(currentPos, startPos));
        
    }
    
    private static void print<T>(T[,] array) {
        for (int j = 0; j < array.GetLength(0); j++) {
            for (int i = 0; i < array.GetLength(1); i++) {
                // 数値型の場合に "00" フォーマットを適用
                if (array[j, i] is IFormattable formattable) {
                    Console.Write($"{formattable.ToString("00", null)},");
                } else {
                    // 数値型以外の場合は通常の ToString を使用
                    Console.Write($"{array[j, i].ToString()},");
                }
            }
            Console.WriteLine("");
        }
    }
    
    public static void test() {
        int[,] track = new int[7,7];
        OutlineTracking(image, track, new Position(2,1), 0);
        print(image);
        Console.WriteLine("Hello World!");
        print(track);
        
    }

}