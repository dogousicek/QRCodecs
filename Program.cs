/*¨bad apple vid https://www.youtube.com/watch?v=9lNZ_Rnr7Jc
 * 
 * 
 * dev note: ITS CREEPY AF HOW THE AI KNOWS WHAT I AM THINKING <- it completed this sentence
 * 
 */

using Raylib_cs;
using System.Diagnostics;
using System.Text;
using static Raylib_cs.Raylib;

namespace QRCodecs;

class Program
{
    public const int MaxInputChars = 53;
    

    // STAThread is required if you deploy using NativeAOT on Windows - See https://github.com/raylib-cs/raylib-cs/issues/301
    [STAThread]
    public static int Main()
    {
        const int screenWidth = 800;
        const int screenHeight = 600;

        InitWindow(screenWidth, screenHeight, "QR code gen in cs");

        char[] name = new char[MaxInputChars];
        int letterCount = 0;

        Rectangle textBox = new(100, 50, 600, 35);
        bool mouseOnText = false;

        int framesCounter = 0;

        Rectangle button = new(400, 100, 200, 50);
        bool mouseOnButton = false;

        SetTargetFPS(20);

        while (!WindowShouldClose())
        {

            if (CheckCollisionPointRec(GetMousePosition(), textBox))
            {
                mouseOnText = true;
            }
            else
            {
                mouseOnText = false;
            }

            if (CheckCollisionPointRec(GetMousePosition(), button))
            {
                mouseOnButton = true;
            }
            else
            {
                 mouseOnButton = false;
            }

            if (mouseOnButton && IsMouseButtonPressed(MouseButton.Left))
            {
                CloseWindow(); // Close the main window
                GenerateQRCode(new string(name));
            }

            if (mouseOnText)
            {
                SetMouseCursor(MouseCursor.IBeam);

                int key = GetCharPressed();

                while (key > 0)
                {
                    // NOTE: Only allow keys in range [32..125]
                    if ((key >= 32) && (key <= 125) && (letterCount < MaxInputChars))
                    {
                        name[letterCount] = (char)key;
                        letterCount++;
                    }

                    // Check next character in the queue
                    key = GetCharPressed();
                }

                if (IsKeyPressed(KeyboardKey.Backspace))
                {
                    letterCount -= 1;
                    if (letterCount < 0)
                    {
                        letterCount = 0;
                    }
                    name[letterCount] = '\0';
                }
            }
            else
            {
                SetMouseCursor(MouseCursor.Default);
            }

            if (mouseOnText)
            {
                framesCounter += 1;
            }
            else
            {
                framesCounter = 0;
            }


            // Draw
            //----------------------------------------------------------------------------------
            BeginDrawing();
            ClearBackground(Color.RayWhite);

            DrawText("Type in the url or whatever", 150, 10, 30, Color.Red);
            DrawRectangleRec(textBox, Color.LightGray);
            DrawRectangleRec(button, Color.LightGray);
            DrawText("GENERATE", (int)button.X + 15, (int)button.Y + 15, 30, Color.Maroon);

            if (mouseOnText)
            {
                DrawRectangleLines(
                    (int)textBox.X,
                    (int)textBox.Y,
                    (int)textBox.Width,
                    (int)textBox.Height,
                    Color.Red
                );
            }
            else
            {
                DrawRectangleLines(
                    (int)textBox.X,
                    (int)textBox.Y,
                    (int)textBox.Width,
                    (int)textBox.Height,
                    Color.DarkGray
                );
            }

            if (mouseOnButton)
            {
                DrawRectangleLines(
                    (int)button.X,
                    (int)button.Y,
                    (int)button.Width,
                    (int)button.Height,
                    Color.Red
                );
            }
            else
            {
                DrawRectangleLines(
                    (int)button.X,
                    (int)button.Y,
                    (int)button.Width,
                    (int)button.Height,
                    Color.DarkGray
                );
            }

            DrawText(new string(name), (int)textBox.X + 5, (int)textBox.Y + 8, 20, Color.Maroon);
            DrawText($"INPUT CHARS: {letterCount}/{MaxInputChars}", 150, 110, 15, Color.Maroon);

            if (mouseOnText)
            {
                if (letterCount < MaxInputChars)
                {
                    // Draw blinking underscore char
                    if ((framesCounter / 20 % 2) == 0)
                    {
                        DrawText(
                            "_",
                            (int)textBox.X + 8 + MeasureText(new string(name), 20),
                            (int)textBox.Y + 12,
                            20,
                            Color.Maroon
                        );
                    }
                }
                else
                {
                    DrawText("Press BACKSPACE to delete chars...", 230, 300, 20, Color.Gray);
                }
            }

            EndDrawing();

        }

        CloseWindow(); // Close window and OpenGL context
        

        return 0;
    }

    //its really freaky how the ai knows shit i want to write, ig i just gotta me the code so bad ai cant do anytihng


    public static void GenerateQRCode(string array)
    {
        int QRSize = 29;
        byte QRVer = 3;



        //GETTING THE QR BITS FROM THE INPUT
        string bitinfo = "0100";    //for bit mode 
        string bitdata = "";        //new string for storing data before assigning the lenght
        int lenght = 0;

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == '\0')
            {
                break;
            }
            bitdata += prevodDoBin((short)array[i]);
            lenght++;
        }
        bitinfo += prevodDoBin(lenght);
        bitinfo += bitdata;     

        bitinfo += "0000 "; //terminator

        
        if(bitinfo.Length / 8 < 53)
        {
            short remaining = (short)(53 - bitinfo.Length / 8);

            Console.WriteLine($"remaining bits: {remaining}");

            while(remaining > 0)
            {
                if(remaining >= 2)
                {
                    bitinfo += "1110110000010001";
                    remaining -= 2;
                } else
                {
                    bitinfo += "11101100";
                    remaining -= 1;
                }
            }
        }

        Console.WriteLine();
        Console.WriteLine(bitinfo);
        Console.WriteLine();
        //------------------



        //MAKE A SEPERATE WINDOW FOR THE QR CODE
        const int qrWindowWidth = 500;
        const int qrWindowHeight = 600;
        const short pixelSize = 12;
        const short Margin = 50;

        InitWindow(qrWindowWidth, qrWindowHeight, "QR Code");
        SetTargetFPS(20);

        while (!WindowShouldClose())
        {
            BeginDrawing();
            ClearBackground(Color.White);

            DrawText(array, 10, 500, 17, Color.Black);  //draws the input text

            // Draw the QR code
            DrawFinder(Margin, Margin, pixelSize);
            DrawFinder(Margin + (QRSize - 7) * pixelSize, Margin, pixelSize);
            DrawFinder(Margin, Margin + (QRSize - 7) * pixelSize, pixelSize);
            DrawAlignment(Margin + 10 * pixelSize + 4 * pixelSize * QRVer, Margin + 10 * pixelSize + 4 * pixelSize * QRVer, pixelSize); // [(10 + 4·v), (10 + 4·v)]
            DrawTiming(Margin + 6 * pixelSize, Margin + 6 * pixelSize, pixelSize);

            DrawOtherStuff(Margin, Margin, pixelSize, QRSize);

            EndDrawing();
        }

        CloseWindow(); // Close window and OpenGL context

        

    }

    /// <summary>
    /// draws finder pattern in the QR code
    /// </summary>
    /// <param name="posx">x pos of the upper left corner</param>
    /// <param name="posy">y pos of the upper left corner</param>
    /// <param name="pixelSize">size of the pixel that it should be drawn with</param>
    public static void DrawFinder(int posx, int posy, int pixelSize)
    {
        DrawRectangle(posx, posy, pixelSize * 7, pixelSize * 7, Color.Black);
        DrawRectangle(posx + pixelSize, posy + pixelSize, pixelSize * 5, pixelSize * 5, Color.White);
        DrawRectangle(posx + pixelSize * 2, posy + pixelSize * 2, pixelSize * 3, pixelSize * 3, Color.Black);

    }

    /// <summary>
    /// draws the alignment pattern in the QR code
    /// </summary>
    /// <param name="posx">x pos of the middle</param>
    /// <param name="posy">y pos of the middle</param>
    /// <param name="pixelSize">size of the pixel that it should be drawn with</param>
    public static void DrawAlignment(int posx, int posy, int pixelSize)
    {
        DrawRectangle(posx - 2 * pixelSize, posy - 2 * pixelSize , pixelSize * 5, pixelSize * 5, Color.Black);

        DrawRectangle(posx - pixelSize, posy - pixelSize, pixelSize * 3, pixelSize * 3, Color.White);

        DrawRectangle(posx, posy, pixelSize, pixelSize, Color.Black);
    }

    public static void DrawTiming(int posx, int posy, int pixelSize)
    {
        for (int i = 0; i <= 14; i++)
        {
            if (i % 2 == 0)
            {
                DrawRectangle(posx + i * pixelSize, posy, pixelSize, pixelSize, Color.Black);
            }
            else
            {
                DrawRectangle(posx + i * pixelSize, posy, pixelSize, pixelSize, Color.White);
            }
        }

        for (int i = 0; i <= 14; i++)
        {
            if (i % 2 == 0)
            {
                DrawRectangle(posx, posy + i * pixelSize, pixelSize, pixelSize, Color.Black);
            }
            else
            {
                DrawRectangle(posx, posy + i * pixelSize, pixelSize, pixelSize, Color.White);
            }
        }
    }

    public static void DrawOtherStuff(int posx, int posy, int pixelSize, int QrSize)
    {
        //drawing THE black pixel
        DrawRectangle(posx + 8 * pixelSize, posy + (QrSize - 8) * pixelSize, pixelSize, pixelSize, Color.Black);
    }

    public static string ReedSolomon(string data, int ecc)
    {
        string codewords = "";  //there will be 55 codewords split into blocks of 15

        //pracujeme v modulo 256
        //zaporna cisla maji stejnou hodnotu jako kladna takze muzeme pouzit abs hodonotu
        //delame XOR operace
        //scititani a odcitani v GF(256) se dela pomoci XOR operaci
        //pouzivame stejne bitwise operatory jako v C tzn. |, ^, &
        //podle qr specificake pouzivame byte-wise modulo 100011101 neboli 285 v desitkove soustave
        //to znaena ze pokud je cislo >= 256 melo by byt XORnuto s 285

        //muzeme nasobit cisla pomoci b^(logb(p) + logb(q)) kde b je ciselna soustava, p a q jsou nasobena cisla

        short codewordCount = 55;



        return codewords;
    }

    public static string Masking(string str)
    {
        return str; // Placeholder for masking logic
    }

    public static void PLOTPOINTS(int posX, int posY, int pixelSize, string data)
    {
        // Placeholder for plotting points logic
        // This function should take the data and plot it on the QR code grid
        // For now, it does nothing
    }

    public static string prevodDoBin(int num)
    {
        string cislo = "";


        while (num > 0)
        {
            if (num % 2 == 0)
            {
                cislo += "0";
            }
            else
            {
                cislo += "1";
            }
            num /= 2;
        }

        string reversedCislo = "";

        for (int i = 0; i < cislo.Length; i++)
        {
            reversedCislo = cislo.ElementAt(i) + reversedCislo;
        }

        while (reversedCislo.Length < 8)
        {
            reversedCislo = "0" + reversedCislo;
        }

        return reversedCislo;
    }

    public static int modulo(double num)
    {
        if(num > 255)
        {
            return (int)num ^ 285; // XOR with 285 for QR code specific masking
        } else
        {
            return (int)num; // No XOR needed for numbers less than 256
        }
            
    }

    public static int multiplyingInGF(int a, int b)
    {
        return (int) Math.Pow(2, (modulo(Math.Log2(a) + Math.Log2(b))));
    }

}
