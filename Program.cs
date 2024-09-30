using System;
using System.IO;
//Tipos de token disponibles
public enum TipoToken
{
    PalabraClave,
    Identificador,
    OperadorL,
    OperadorM,
    Numero,
    Delimitador,
    Comentario,
    Desconocido
}
//Clase programa 
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Introduce el código fuente:");
        string codigo = "";
        string linea;

        //Leer varias lineas en la consola
        while((linea=Console.ReadLine()) != ""){
            codigo += linea + "\n";//Concatenar las lineas
        }
        Console.WriteLine("Código fuente recibido: ");
        Console.WriteLine(codigo);
        List<Token> tokens = AnalizadorLexico.Analizar(codigo);

        // Encabezado de la tabla
        Console.WriteLine("\nTabla de Tokens:");
        Console.WriteLine("{0,-10} | {1,-15} | {2,-15}", "Índice", "Tipo de Token", "Valor");
        Console.WriteLine(new string('-', 45));

        // Imprimir los tokens
        for (int i = 0; i < tokens.Count; i++)
        {
            Console.WriteLine("{0,-10} | {1,-15} | {2,-15}", i, tokens[i].Tipo, tokens[i].Valor);
        }
    }
}
