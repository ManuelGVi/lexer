using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

// Tipos de token disponibles
public enum TipoToken
{
    PalabraClave,
    Identificador,
    Operador,
    Numero,
    Delimitador,
    Comentario,
    Desconocido
}

// Clase Token que representa cada unidad léxica
public class Token
{
    public TipoToken Tipo { get; set; }
    public string Valor { get; set; }

    public Token(TipoToken tipo, string valor)
    {
        Tipo = tipo;
        Valor = valor;
    }

    public override string ToString()
    {
        return $"[{Tipo}, '{Valor}']";
    }
}

// Clase AnalizadorLexico que realiza el proceso de análisis
public class AnalizadorLexico
{
    // Palabras clave del lenguaje
    private static readonly string[] palabrasClave = { "si", "sino", "mientras", "retorno" };
    
    // Operadores del lenguaje
    private static readonly string[] operadores = { "+", "-", "*", "/", "=", "==" };
    
    // Delimitadores del lenguaje
    private static readonly string[] delimitadores = { "(", ")", "{", "}", ";" };

    // Expresiones regulares para números e identificadores
    private static readonly Regex patronNumero = new Regex(@"^\d+(\.\d+)?");
    private static readonly Regex patronIdentificador = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*");

    // Método para analizar el código fuente
    public static List<Token> Analizar(string codigoFuente)
    {
        List<Token> tokens = new List<Token>();
        int posicion = 0;

        while (posicion < codigoFuente.Length)
        {
            char caracterActual = codigoFuente[posicion];

            // Ignorar espacios en blanco
            if (char.IsWhiteSpace(caracterActual))
            {
                posicion++;
                continue;
            }

            // Comentarios de una línea
            if (codigoFuente.Substring(posicion).StartsWith("//"))
            {
                int finComentario = codigoFuente.IndexOf('\n', posicion);
                if (finComentario == -1) finComentario = codigoFuente.Length;
                string comentario = codigoFuente.Substring(posicion, finComentario - posicion);
                tokens.Add(new Token(TipoToken.Comentario, comentario));
                posicion = finComentario;
                continue;
            }

            // Comentarios de múltiples líneas
            if (codigoFuente.Substring(posicion).StartsWith("/*"))
            {
                int finComentario = codigoFuente.IndexOf("*/", posicion);
                if (finComentario == -1) throw new Exception("Comentario sin cerrar");
                string comentario = codigoFuente.Substring(posicion, finComentario - posicion + 2);
                tokens.Add(new Token(TipoToken.Comentario, comentario));
                posicion = finComentario + 2;
                continue;
            }

            // Palabras clave o identificadores
            Match coincidenciaIdentificador = patronIdentificador.Match(codigoFuente.Substring(posicion));
            if (coincidenciaIdentificador.Success)
            {
                string valor = coincidenciaIdentificador.Value;
                TipoToken tipo = Array.Exists(palabrasClave, palabra => palabra == valor) ? TipoToken.PalabraClave : TipoToken.Identificador;
                tokens.Add(new Token(tipo, valor));
                posicion += valor.Length;
                continue;
            }

            // Números (enteros o decimales)
            Match coincidenciaNumero = patronNumero.Match(codigoFuente.Substring(posicion));
            if (coincidenciaNumero.Success)
            {
                tokens.Add(new Token(TipoToken.Numero, coincidenciaNumero.Value));
                posicion += coincidenciaNumero.Value.Length;
                continue;
            }

            // Operadores
            foreach (string operador in operadores)
            {
                if (codigoFuente.Substring(posicion).StartsWith(operador))
                {
                    tokens.Add(new Token(TipoToken.Operador, operador));
                    posicion += operador.Length;
                    break;
                }
            }

            // Delimitadores
            foreach (string delimitador in delimitadores)
            {
                if (codigoFuente[posicion].ToString() == delimitador)
                {
                    tokens.Add(new Token(TipoToken.Delimitador, delimitador));
                    posicion++;
                    break;
                }
            }

            // Si no se reconoce el carácter, marcarlo como desconocido
            if (tokens.Count == 0 || posicion == 0)
            {
                tokens.Add(new Token(TipoToken.Desconocido, caracterActual.ToString()));
                posicion++;
            }
        }

        return tokens;
    }
}

// Clase principal que ejecuta el programa
class Programa
{
    static void Main(string[] args)
    {
        string rutaArchivo = "programa1.txt"; // Ruta del archivo de código fuente

        if (!File.Exists(rutaArchivo))
        {
            Console.WriteLine("Archivo no encontrado.");
            return;
        }

        string codigoFuente = File.ReadAllText(rutaArchivo);
        List<Token> tokens = AnalizadorLexico.Analizar(codigoFuente);

        foreach (Token token in tokens)
        {
            Console.WriteLine(token);
        }
    }
}
