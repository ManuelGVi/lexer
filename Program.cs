using System;
using System.Collections.Generic;

namespace AnalizadorLexico
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Introduce el código fuente:");
            string codigo = "";
            string linea;

            // Leer varias líneas hasta que el usuario presione Enter sin texto
            while ((linea = Console.ReadLine()) != "")
            {
                codigo += linea + "\n"; // Concatenar las líneas
            }

            // Mostrar el código fuente introducido
            Console.WriteLine("Código fuente recibido:");
            Console.WriteLine(codigo);

            // Análisis léxico: generar los tokens a partir del código fuente
            List<Token> tokens = Lexer.Analizar(codigo);

            // Imprimir la tabla de tokens
            Console.WriteLine("\nTabla de Tokens:");
            Console.WriteLine("{0,-10} | {1,-15} | {2,-15}", "Índice", "Tipo de Token", "Valor");
            Console.WriteLine(new string('-', 45));

            for (int i = 0; i < tokens.Count; i++)
            {
                Console.WriteLine("{0,-10} | {1,-15} | {2,-15}", i, tokens[i].Tipo, tokens[i].Valor);
            }

            // Análisis sintáctico: generar el árbol sintáctico a partir de los tokens
            AnalizadorSintactico analizador = new AnalizadorSintactico(tokens);
            Nodo arbolSintactico = analizador.Analizar();

            // Mostrar el árbol sintáctico
            Console.WriteLine("\nÁrbol Sintáctico:");
            arbolSintactico.Imprimir();
            GeneradorCodigoIntermedio generador = new GeneradorCodigoIntermedio();
            List<Cuadruplo> cuadruplos = generador.GenerarCodigoIntermedio(arbolSintactico);

            // Mostrar los cuádruplos generados
            Console.WriteLine("\nCódigo Intermedio (Cuádruplos):");
            foreach (var cuadruplo in cuadruplos)
            {
                Console.WriteLine(cuadruplo);
            }
            GenerarCodigo generadorCodigo = new GenerarCodigo(cuadruplos);
            generadorCodigo.Generar();
        }
    }
}
