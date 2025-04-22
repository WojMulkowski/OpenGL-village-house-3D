using GLFW;
using GlmSharp;

using Shaders;

using OpenTK;
using OpenTK.Graphics.OpenGL4;

using System.Drawing;
using System.Collections.Generic;
using System.IO;
using ObjLoader.Loader.Loaders;

namespace PMLabs
{
    // Implementacja interfejsu dostosowującego metodę biblioteki Glfw służącą do pozyskiwania adresów funkcji i procedur OpenGL do współpracy z OpenTK.
    public class BC : IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            return Glfw.GetProcAddress(procName);
        }
    }

    class Program
    {
        static ShaderProgram shader;
        static float speed_y;
        static float speed_x;

        static int texDach;
        static int texDom;
        static int texDrzwi;
        static int texKomin;
        static int texOkno;
        static int texTrawa;
        static int texNiebo;

        static Model modelDach;
        static Model modelDom1;
        static Model modelDom2;
        static Model modelDrzwi;
        static Model modelKomin;
        static Model modelOkno1;
        static Model modelOkno2;
        static Model modelOkno3;
        static Model modelOkno4;
        static Model modelTrawa;
        static Model modelNiebo;

        static KeyCallback kc = KeyProcessor;

        public static void KeyProcessor(System.IntPtr window, Keys key, int scanCode, InputState state, ModifierKeys mods)
        {
            if (state == InputState.Press)
            {
                if (key == Keys.Left) speed_y = -3.14f;
                if (key == Keys.Right) speed_y = 3.14f;
                if (key == Keys.Up) speed_x = -3.14f;
                if (key == Keys.Down) speed_x = 3.14f;
            }
            if (state == InputState.Release)
            {
                if (key == Keys.Left) speed_y = 0;
                if (key == Keys.Right) speed_y = 0;
                if (key == Keys.Up) speed_x = 0;
                if (key == Keys.Down) speed_x = 0;
            }
        }

        public static void InitOpenGLProgram(Window window)
        {
            GL.ClearColor(0, 0, 0, 1);
            shader = new ShaderProgram("vertex_shader.glsl", "fragment_shader.glsl");

            // Inicjalizacja wczytywania modelu
            ObjModelLoader loader = new ObjModelLoader();
            modelDach = new Model();
            loader.Load("dach.obj", modelDach); // Podaj ścieżkę do pliku OBJ
            modelDom1 = new Model();
            loader.Load("dom_szescian.obj", modelDom1);
            modelDom2 = new Model();
            loader.Load("dom_szescian.obj", modelDom2);
            modelDrzwi = new Model();
            loader.Load("drzwi.obj", modelDrzwi);
            modelKomin = new Model();
            loader.Load("komin.obj", modelKomin);
            modelOkno1 = new Model();
            loader.Load("okno.obj", modelOkno1);
            modelOkno2 = new Model();
            loader.Load("okno.obj", modelOkno2);
            modelOkno3 = new Model();
            loader.Load("okno.obj", modelOkno3);
            modelOkno4 = new Model();
            loader.Load("okno.obj", modelOkno4);
            modelTrawa = new Model();
            loader.Load("dom_szescian.obj", modelTrawa);
            modelNiebo = new Model();
            loader.Load("dom_szescian.obj", modelNiebo);


            texDach = ReadTexture("Tekstury\\dach_texture.jpg");
            texDom = ReadTexture("Tekstury\\dom_texture.jpg", TextureUnit.Texture1);
            texDrzwi = ReadTexture("Tekstury\\drzwi_texture.jpg", TextureUnit.Texture2);
            texKomin = ReadTexture("Tekstury\\komin_texture.jpg", TextureUnit.Texture3);
            texOkno = ReadTexture("Tekstury\\okno_texture.jpg", TextureUnit.Texture4);
            texTrawa = ReadTexture("Tekstury\\trawa_texture.jpg", TextureUnit.Texture5);
            texNiebo = ReadTexture("Tekstury\\niebo_texture.jpg", TextureUnit.Texture6);

            Glfw.SetKeyCallback(window, kc);
            GL.Enable(EnableCap.DepthTest); 
        }

        public static void FreeOpenGLProgram(Window window)
        {
            GL.DeleteTexture(texDach);
            GL.DeleteTexture(texDom);
            GL.DeleteTexture(texDrzwi);
            GL.DeleteTexture(texKomin);
            GL.DeleteTexture(texOkno);
            GL.DeleteTexture(texTrawa);
            GL.DeleteTexture(texNiebo);
        }

        //MODYFIKACJA. Ta wersja funkcji pozwala łatwo wczytać teksturę do innej jednostki teksturującej - należy ją podać jako argument.
        public static int ReadTexture(string filename, TextureUnit textureUnit = TextureUnit.Texture0)
        {
            var tex = GL.GenTexture();
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            Bitmap bitmap = new Bitmap(filename);
            System.Drawing.Imaging.BitmapData data = bitmap.LockBits(
              new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
              System.Drawing.Imaging.ImageLockMode.ReadOnly,
              System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width,
              data.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);
            bitmap.Dispose();

            GL.TexParameter(TextureTarget.Texture2D,
              TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
              TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

            return tex;
        }

        public static void DrawScene(Window window, float angle_x, float angle_y)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            mat4 P = mat4.Perspective(glm.Radians(70.0f), 1, 1, 70);
            mat4 V = mat4.LookAt(new vec3(0, 0, -9), new vec3(0, 0, 0), new vec3(0, 1, 0));

            shader.Use();
            GL.UniformMatrix4(shader.U("P"), 1, false, P.Values1D);
            GL.UniformMatrix4(shader.U("V"), 1, false, V.Values1D);

            // MODEL KOMIN
            mat4 kominM = mat4.Rotate(angle_y, new vec3(0, 1, 0)) * mat4.Rotate(angle_x, new vec3(1, 0, 0));
            kominM *= mat4.Scale(new vec3(0.1f, 0.22f, 0.15f));
            kominM *= mat4.Translate(new vec3(8.0f, 6.0f, 1.0f));
            GL.UniformMatrix4(shader.U("M"), 1, false, kominM.Values1D);

            GL.Uniform1(shader.U("tex"), 3);  // Przekaż indeks jednostki tekstury do shadera

            GL.EnableVertexAttribArray(shader.A("vertex"));
            GL.EnableVertexAttribArray(shader.A("normal"));
            GL.EnableVertexAttribArray(shader.A("texCoord"));
            GL.EnableVertexAttribArray(shader.A("color"));

            GL.VertexAttribPointer(shader.A("vertex"), 4, VertexAttribPointerType.Float, false, 0, modelKomin.vertices.ToArray());
            GL.VertexAttribPointer(shader.A("normal"), 4, VertexAttribPointerType.Float, false, 0, modelKomin.vertexNormals.ToArray());
            GL.VertexAttribPointer(shader.A("texCoord"), 2, VertexAttribPointerType.Float, false, 0, modelKomin.texCoords.ToArray());
            GL.VertexAttribPointer(shader.A("color"), 4, VertexAttribPointerType.Float, false, 0, modelKomin.normals.ToArray());

            GL.DrawArrays(PrimitiveType.QuadsExt, 0, modelKomin.vertexCount);

            GL.DisableVertexAttribArray(shader.A("vertex"));
            GL.DisableVertexAttribArray(shader.A("normal"));
            GL.DisableVertexAttribArray(shader.A("texCoord"));
            GL.DisableVertexAttribArray(shader.A("color"));

            // MODEL DACH
            mat4 dachM = mat4.Rotate(angle_y, new vec3(0, 1, 0)) * mat4.Rotate(angle_x, new vec3(1, 0, 0));
            dachM *= mat4.Translate(new vec3(0.5f, 0.30f, 0.5f));
            GL.UniformMatrix4(shader.U("M"), 1, false, dachM.Values1D);

            GL.Uniform1(shader.U("tex"), 0);  // Przekaż indeks jednostki tekstury do shadera

            GL.EnableVertexAttribArray(shader.A("vertex"));
            GL.EnableVertexAttribArray(shader.A("normal"));
            GL.EnableVertexAttribArray(shader.A("texCoord"));
            GL.EnableVertexAttribArray(shader.A("color"));

            GL.VertexAttribPointer(shader.A("vertex"), 4, VertexAttribPointerType.Float, false, 0, modelDach.vertices.ToArray());
            GL.VertexAttribPointer(shader.A("normal"), 4, VertexAttribPointerType.Float, false, 0, modelDach.vertexNormals.ToArray());
            GL.VertexAttribPointer(shader.A("texCoord"), 2, VertexAttribPointerType.Float, false, 0, modelDach.texCoords.ToArray());
            GL.VertexAttribPointer(shader.A("color"), 4, VertexAttribPointerType.Float, false, 0, modelDach.normals.ToArray());

            GL.DrawArrays(PrimitiveType.QuadsExt, 0, modelDach.vertexCount);

            GL.DisableVertexAttribArray(shader.A("vertex"));
            GL.DisableVertexAttribArray(shader.A("normal"));
            GL.DisableVertexAttribArray(shader.A("texCoord"));
            GL.DisableVertexAttribArray(shader.A("color"));

            // MODEL DOM_1
            mat4 dom1M = mat4.Rotate(angle_y, new vec3(0, 1, 0)) * mat4.Rotate(angle_x, new vec3(1, 0, 0));
            dom1M *= mat4.Translate(new vec3(0.0f, 0.0f, 0.0f));
            GL.UniformMatrix4(shader.U("M"), 1, false, dom1M.Values1D);

            GL.Uniform1(shader.U("tex"), 1);  // Przekaż indeks jednostki tekstury do shadera

            GL.EnableVertexAttribArray(shader.A("vertex"));
            GL.EnableVertexAttribArray(shader.A("normal"));
            GL.EnableVertexAttribArray(shader.A("texCoord"));
            GL.EnableVertexAttribArray(shader.A("color"));

            GL.VertexAttribPointer(shader.A("vertex"), 4, VertexAttribPointerType.Float, false, 0, modelDom1.vertices.ToArray());
            GL.VertexAttribPointer(shader.A("normal"), 4, VertexAttribPointerType.Float, false, 0, modelDom1.vertexNormals.ToArray());
            GL.VertexAttribPointer(shader.A("texCoord"), 2, VertexAttribPointerType.Float, false, 0, modelDom1.texCoords.ToArray());
            GL.VertexAttribPointer(shader.A("color"), 4, VertexAttribPointerType.Float, false, 0, modelDom1.normals.ToArray());

            GL.DrawArrays(PrimitiveType.QuadsExt, 0, modelDom1.vertexCount);

            GL.DisableVertexAttribArray(shader.A("vertex"));
            GL.DisableVertexAttribArray(shader.A("normal"));
            GL.DisableVertexAttribArray(shader.A("texCoord"));
            GL.DisableVertexAttribArray(shader.A("color"));

            // MODEL DOM_2
            mat4 dom2M = mat4.Rotate(angle_y, new vec3(0, 1, 0)) * mat4.Rotate(angle_x, new vec3(1, 0, 0));
            dom2M *= mat4.Translate(new vec3(0.0f, -2.0f, 0.0f));
            GL.UniformMatrix4(shader.U("M"), 1, false, dom2M.Values1D);

            GL.Uniform1(shader.U("tex"), 1);  // Przekaż indeks jednostki tekstury do shadera

            GL.EnableVertexAttribArray(shader.A("vertex"));
            GL.EnableVertexAttribArray(shader.A("normal"));
            GL.EnableVertexAttribArray(shader.A("texCoord"));
            GL.EnableVertexAttribArray(shader.A("color"));

            GL.VertexAttribPointer(shader.A("vertex"), 4, VertexAttribPointerType.Float, false, 0, modelDom2.vertices.ToArray());
            GL.VertexAttribPointer(shader.A("normal"), 4, VertexAttribPointerType.Float, false, 0, modelDom2.vertexNormals.ToArray());
            GL.VertexAttribPointer(shader.A("texCoord"), 2, VertexAttribPointerType.Float, false, 0, modelDom2.texCoords.ToArray());
            GL.VertexAttribPointer(shader.A("color"), 4, VertexAttribPointerType.Float, false, 0, modelDom2.normals.ToArray());

            GL.DrawArrays(PrimitiveType.QuadsExt, 0, modelDom2.vertexCount);

            GL.DisableVertexAttribArray(shader.A("vertex"));
            GL.DisableVertexAttribArray(shader.A("normal"));
            GL.DisableVertexAttribArray(shader.A("texCoord"));
            GL.DisableVertexAttribArray(shader.A("color"));

            // MODEL DRZWI
            mat4 drzwiM = mat4.Rotate(angle_y + (-136.6f), new vec3(0, 1, 0)) * mat4.Rotate(angle_x, new vec3(1, 0, 0));
            drzwiM *= mat4.Scale(new vec3(0.2f, 0.12f, 0.12f));
            drzwiM *= mat4.Translate(new vec3(8.0f, -23.1f, -1.5f));
            GL.UniformMatrix4(shader.U("M"), 1, false, drzwiM.Values1D);

            GL.Uniform1(shader.U("tex"), 2);  // Przekaż indeks jednostki tekstury do shadera

            GL.EnableVertexAttribArray(shader.A("vertex"));
            GL.EnableVertexAttribArray(shader.A("normal"));
            GL.EnableVertexAttribArray(shader.A("texCoord"));
            GL.EnableVertexAttribArray(shader.A("color"));

            GL.VertexAttribPointer(shader.A("vertex"), 4, VertexAttribPointerType.Float, false, 0, modelDrzwi.vertices.ToArray());
            GL.VertexAttribPointer(shader.A("normal"), 4, VertexAttribPointerType.Float, false, 0, modelDrzwi.vertexNormals.ToArray());
            GL.VertexAttribPointer(shader.A("texCoord"), 2, VertexAttribPointerType.Float, false, 0, modelDrzwi.texCoords.ToArray());
            GL.VertexAttribPointer(shader.A("color"), 4, VertexAttribPointerType.Float, false, 0, modelDrzwi.normals.ToArray());

            GL.DrawArrays(PrimitiveType.QuadsExt, 0, modelDrzwi.vertexCount);

            GL.DisableVertexAttribArray(shader.A("vertex"));
            GL.DisableVertexAttribArray(shader.A("normal"));
            GL.DisableVertexAttribArray(shader.A("texCoord"));
            GL.DisableVertexAttribArray(shader.A("color"));

            // MODEL OKNO_1
            mat4 okno1M = mat4.Rotate(angle_y + (-136.6f), new vec3(0, 1, 0)) * mat4.Rotate(angle_x, new vec3(1, 0, 0));
            okno1M *= mat4.Scale(new vec3(0.2f, 0.16f, 0.115f));
            okno1M *= mat4.Translate(new vec3(8.0f, -4.0f, -1.5f));
            GL.UniformMatrix4(shader.U("M"), 1, false, okno1M.Values1D);

            GL.Uniform1(shader.U("tex"), 4);  // Przekaż indeks jednostki tekstury do shadera

            GL.EnableVertexAttribArray(shader.A("vertex"));
            GL.EnableVertexAttribArray(shader.A("normal"));
            GL.EnableVertexAttribArray(shader.A("texCoord"));
            GL.EnableVertexAttribArray(shader.A("color"));

            GL.VertexAttribPointer(shader.A("vertex"), 4, VertexAttribPointerType.Float, false, 0, modelOkno1.vertices.ToArray());
            GL.VertexAttribPointer(shader.A("normal"), 4, VertexAttribPointerType.Float, false, 0, modelOkno1.vertexNormals.ToArray());
            GL.VertexAttribPointer(shader.A("texCoord"), 2, VertexAttribPointerType.Float, false, 0, modelOkno1.texCoords.ToArray());
            GL.VertexAttribPointer(shader.A("color"), 4, VertexAttribPointerType.Float, false, 0, modelOkno1.normals.ToArray());

            GL.DrawArrays(PrimitiveType.QuadsExt, 0, modelOkno1.vertexCount);

            GL.DisableVertexAttribArray(shader.A("vertex"));
            GL.DisableVertexAttribArray(shader.A("normal"));
            GL.DisableVertexAttribArray(shader.A("texCoord"));
            GL.DisableVertexAttribArray(shader.A("color"));

            // MODEL OKNO_2
            mat4 okno2M = mat4.Rotate(angle_y + (136.6f), new vec3(0, 1, 0)) * mat4.Rotate(angle_x, new vec3(1, 0, 0));
            okno2M *= mat4.Scale(new vec3(0.2f, 0.16f, 0.115f));
            okno2M *= mat4.Translate(new vec3(8.0f, -4.0f, -2.5f));
            GL.UniformMatrix4(shader.U("M"), 1, false, okno2M.Values1D);

            GL.Uniform1(shader.U("tex"), 4);  // Przekaż indeks jednostki tekstury do shadera

            GL.EnableVertexAttribArray(shader.A("vertex"));
            GL.EnableVertexAttribArray(shader.A("normal"));
            GL.EnableVertexAttribArray(shader.A("texCoord"));
            GL.EnableVertexAttribArray(shader.A("color"));

            GL.VertexAttribPointer(shader.A("vertex"), 4, VertexAttribPointerType.Float, false, 0, modelOkno2.vertices.ToArray());
            GL.VertexAttribPointer(shader.A("normal"), 4, VertexAttribPointerType.Float, false, 0, modelOkno2.vertexNormals.ToArray());
            GL.VertexAttribPointer(shader.A("texCoord"), 2, VertexAttribPointerType.Float, false, 0, modelOkno2.texCoords.ToArray());
            GL.VertexAttribPointer(shader.A("color"), 4, VertexAttribPointerType.Float, false, 0, modelOkno2.normals.ToArray());

            GL.DrawArrays(PrimitiveType.QuadsExt, 0, modelOkno2.vertexCount);

            GL.DisableVertexAttribArray(shader.A("vertex"));
            GL.DisableVertexAttribArray(shader.A("normal"));
            GL.DisableVertexAttribArray(shader.A("texCoord"));
            GL.DisableVertexAttribArray(shader.A("color"));

            // MODEL OKNO_3
            mat4 okno3M = mat4.Rotate(angle_y + (69.0f), new vec3(0, 1, 0)) * mat4.Rotate(angle_x, new vec3(1, 0, 0));
            okno3M *= mat4.Scale(new vec3(0.2f, 0.16f, 0.115f));
            okno3M *= mat4.Translate(new vec3(8.0f, -4.0f, -3f));
            GL.UniformMatrix4(shader.U("M"), 1, false, okno3M.Values1D);

            GL.Uniform1(shader.U("tex"), 4);  // Przekaż indeks jednostki tekstury do shadera

            GL.EnableVertexAttribArray(shader.A("vertex"));
            GL.EnableVertexAttribArray(shader.A("normal"));
            GL.EnableVertexAttribArray(shader.A("texCoord"));
            GL.EnableVertexAttribArray(shader.A("color"));

            GL.VertexAttribPointer(shader.A("vertex"), 4, VertexAttribPointerType.Float, false, 0, modelOkno3.vertices.ToArray());
            GL.VertexAttribPointer(shader.A("normal"), 4, VertexAttribPointerType.Float, false, 0, modelOkno3.vertexNormals.ToArray());
            GL.VertexAttribPointer(shader.A("texCoord"), 2, VertexAttribPointerType.Float, false, 0, modelOkno3.texCoords.ToArray());
            GL.VertexAttribPointer(shader.A("color"), 4, VertexAttribPointerType.Float, false, 0, modelOkno3.normals.ToArray());

            GL.DrawArrays(PrimitiveType.QuadsExt, 0, modelOkno3.vertexCount);

            GL.DisableVertexAttribArray(shader.A("vertex"));
            GL.DisableVertexAttribArray(shader.A("normal"));
            GL.DisableVertexAttribArray(shader.A("texCoord"));
            GL.DisableVertexAttribArray(shader.A("color"));

            // MODEL OKNO_4
            mat4 okno4M = mat4.Rotate(angle_y + (-204.2f), new vec3(0, 1, 0)) * mat4.Rotate(angle_x, new vec3(1, 0, 0));
            okno4M *= mat4.Scale(new vec3(0.2f, 0.16f, 0.115f));
            okno4M *= mat4.Translate(new vec3(8.0f, -4.0f, -2.0f));
            GL.UniformMatrix4(shader.U("M"), 1, false, okno4M.Values1D);

            GL.Uniform1(shader.U("tex"), 4);  // Przekaż indeks jednostki tekstury do shadera

            GL.EnableVertexAttribArray(shader.A("vertex"));
            GL.EnableVertexAttribArray(shader.A("normal"));
            GL.EnableVertexAttribArray(shader.A("texCoord"));
            GL.EnableVertexAttribArray(shader.A("color"));

            GL.VertexAttribPointer(shader.A("vertex"), 4, VertexAttribPointerType.Float, false, 0, modelOkno4.vertices.ToArray());
            GL.VertexAttribPointer(shader.A("normal"), 4, VertexAttribPointerType.Float, false, 0, modelOkno4.vertexNormals.ToArray());
            GL.VertexAttribPointer(shader.A("texCoord"), 2, VertexAttribPointerType.Float, false, 0, modelOkno4.texCoords.ToArray());
            GL.VertexAttribPointer(shader.A("color"), 4, VertexAttribPointerType.Float, false, 0, modelOkno4.normals.ToArray());

            GL.DrawArrays(PrimitiveType.QuadsExt, 0, modelOkno4.vertexCount);

            GL.DisableVertexAttribArray(shader.A("vertex"));
            GL.DisableVertexAttribArray(shader.A("normal"));
            GL.DisableVertexAttribArray(shader.A("texCoord"));
            GL.DisableVertexAttribArray(shader.A("color"));

            // MODEL TRAWA
            mat4 trawaM = mat4.Rotate(0, new vec3(0, 1, 0)) * mat4.Rotate(0, new vec3(1, 0, 0));
            trawaM *= mat4.Scale(new vec3(30.0f, 1.0f, 25.0f));
            trawaM *= mat4.Translate(new vec3(0.0f, -4.35f, 0.0f));
            GL.UniformMatrix4(shader.U("M"), 1, false, trawaM.Values1D);

            GL.Uniform1(shader.U("tex"), 5);  // Przekaż indeks jednostki tekstury do shadera

            GL.EnableVertexAttribArray(shader.A("vertex"));
            GL.EnableVertexAttribArray(shader.A("normal"));
            GL.EnableVertexAttribArray(shader.A("texCoord"));
            GL.EnableVertexAttribArray(shader.A("color"));

            GL.VertexAttribPointer(shader.A("vertex"), 4, VertexAttribPointerType.Float, false, 0, modelTrawa.vertices.ToArray());
            GL.VertexAttribPointer(shader.A("normal"), 4, VertexAttribPointerType.Float, false, 0, modelTrawa.vertexNormals.ToArray());
            GL.VertexAttribPointer(shader.A("texCoord"), 2, VertexAttribPointerType.Float, false, 0, modelTrawa.texCoords.ToArray());
            GL.VertexAttribPointer(shader.A("color"), 4, VertexAttribPointerType.Float, false, 0, modelTrawa.normals.ToArray());

            GL.DrawArrays(PrimitiveType.QuadsExt, 0, modelTrawa.vertexCount);

            GL.DisableVertexAttribArray(shader.A("vertex"));
            GL.DisableVertexAttribArray(shader.A("normal"));
            GL.DisableVertexAttribArray(shader.A("texCoord"));
            GL.DisableVertexAttribArray(shader.A("color"));

            // MODEL NIEBO
            mat4 nieboM = mat4.Rotate(0, new vec3(0, 1, 0)) * mat4.Rotate(0, new vec3(1, 0, 0));
            nieboM *= mat4.Scale(new vec3(50.0f, 35.0f, 12.0f));
            nieboM *= mat4.Translate(new vec3(0.0f, 0.0f, 5.0f));
            GL.UniformMatrix4(shader.U("M"), 1, false, nieboM.Values1D);

            GL.Uniform1(shader.U("tex"), 6);  // Przekaż indeks jednostki tekstury do shadera

            GL.EnableVertexAttribArray(shader.A("vertex"));
            GL.EnableVertexAttribArray(shader.A("normal"));
            GL.EnableVertexAttribArray(shader.A("texCoord"));
            GL.EnableVertexAttribArray(shader.A("color"));

            GL.VertexAttribPointer(shader.A("vertex"), 4, VertexAttribPointerType.Float, false, 0, modelNiebo.vertices.ToArray());
            GL.VertexAttribPointer(shader.A("normal"), 4, VertexAttribPointerType.Float, false, 0, modelNiebo.vertexNormals.ToArray());
            GL.VertexAttribPointer(shader.A("texCoord"), 2, VertexAttribPointerType.Float, false, 0, modelNiebo.texCoords.ToArray());
            GL.VertexAttribPointer(shader.A("color"), 4, VertexAttribPointerType.Float, false, 0, modelNiebo.normals.ToArray());

            GL.DrawArrays(PrimitiveType.QuadsExt, 0, modelNiebo.vertexCount);

            GL.DisableVertexAttribArray(shader.A("vertex"));
            GL.DisableVertexAttribArray(shader.A("normal"));
            GL.DisableVertexAttribArray(shader.A("texCoord"));
            GL.DisableVertexAttribArray(shader.A("color"));

            Glfw.SwapBuffers(window);
        }

        static void Main(string[] args)
        {
            Glfw.Init();//Zainicjuj bibliotekę GLFW

            Window window = Glfw.CreateWindow(500, 500, "OpenGL", GLFW.Monitor.None, Window.None);

            Glfw.MakeContextCurrent(window);
            Glfw.SwapInterval(1);

            GL.LoadBindings(new BC());

            InitOpenGLProgram(window);

            Glfw.Time = 0;

            float angle_x = 0;
            float angle_y = 0;

            while (!Glfw.WindowShouldClose(window))
            {
                angle_x += speed_x * (float)Glfw.Time; //Aktualizuj kat obrotu wokół osi X zgodnie z prędkością obrotu
                angle_y += speed_y * (float)Glfw.Time; //Aktualizuj kat obrotu wokół osi Y zgodnie z prędkością obrotu
                Glfw.Time = 0; //Wyzeruj licznik czasu
                DrawScene(window, angle_x, angle_y);

                Glfw.PollEvents();
            }


            FreeOpenGLProgram(window);

            Glfw.Terminate();
        }
    }
}
