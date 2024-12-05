using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Lexer
{
    // Palabras clave del lenguaje
    private static readonly string[] palabrasClave = { "si", "sino", "mientras", "retorno", "Programa","para" };
    // Operadores lógicos
    private static readonly string[] operadoresl = { "==", "<=", ">=","!=","<",">"};
    // Operadores matemáticos
    private static readonly string[] operadoresm = { "+", "-", "*", "/", "=" };
    
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
        int linea = 1;
        int columna = 1;

        while (posicion < codigoFuente.Length)
        {
            char caracterActual = codigoFuente[posicion];

            // Ignorar espacios en blanco
            if (char.IsWhiteSpace(caracterActual))
            {
                if (caracterActual == '\n')
                {
                    linea++;
                    columna = 1;
                }
                else
                {
                    columna++;
                }
                posicion++;
                continue;
            }

            // Comentarios de una línea
            if (codigoFuente.Substring(posicion).StartsWith("//"))
            {
                int finComentario = codigoFuente.IndexOf('\n', posicion);
                if (finComentario == -1) finComentario = codigoFuente.Length;
                string comentario = codigoFuente.Substring(posicion, finComentario - posicion);
                tokens.Add(new Token(TipoToken.Comentario, comentario, linea, columna));
                posicion = finComentario;
                columna = 1;
                linea++;
                continue;
            }

            // Comentarios de múltiples líneas
            if (codigoFuente.Substring(posicion).StartsWith("/*"))
            {
                int finComentario = codigoFuente.IndexOf("*/", posicion);
                if (finComentario == -1) throw new Exception("Comentario sin cerrar");
                string comentario = codigoFuente.Substring(posicion, finComentario - posicion + 2);
                tokens.Add(new Token(TipoToken.Comentario, comentario, linea, columna));
                posicion = finComentario + 2;
                // Actualiza la línea y columna después del comentario
                linea = codigoFuente.Substring(0, posicion).Count(c => c == '\n') + 1;
                columna = posicion - codigoFuente.LastIndexOf('\n', posicion - 1);
                continue;
            }

            // Palabras clave o identificadores
            Match coincidenciaIdentificador = patronIdentificador.Match(codigoFuente.Substring(posicion));
            if (coincidenciaIdentificador.Success)
            {
                string valor = coincidenciaIdentificador.Value;
                TipoToken tipo = Array.Exists(palabrasClave, palabra => palabra == valor) ? TipoToken.PalabraClave : TipoToken.Identificador;
                tokens.Add(new Token(tipo, valor, linea, columna));
                posicion += valor.Length;
                columna += valor.Length;
                continue;
            }

            // Números (enteros o decimales)
            Match coincidenciaNumero = patronNumero.Match(codigoFuente.Substring(posicion));
            if (coincidenciaNumero.Success)
            {
                tokens.Add(new Token(TipoToken.Numero, coincidenciaNumero.Value, linea, columna));
                posicion += coincidenciaNumero.Value.Length;
                columna += coincidenciaNumero.Value.Length;
                continue;
            }

            // Operadores lógicos
            foreach (string operadorl in operadoresl)
            {
                if (codigoFuente.Substring(posicion).StartsWith(operadorl))
                {
                    tokens.Add(new Token(TipoToken.OperadorL, operadorl, linea, columna));
                    posicion += operadorl.Length;
                    columna += operadorl.Length;
                    break;
                }
            }

            // Operadores matemáticos
            foreach (string operadorm in operadoresm)
            {
                if (codigoFuente.Substring(posicion).StartsWith(operadorm))
                {
                    tokens.Add(new Token(TipoToken.OperadorM, operadorm, linea, columna));
                    posicion += operadorm.Length;
                    columna += operadorm.Length;
                    break;
                }
            }

            // Delimitadores
            foreach (string delimitador in delimitadores)
            {
                if (codigoFuente[posicion].ToString() == delimitador)
                {
                    tokens.Add(new Token(TipoToken.Delimitador, delimitador, linea, columna));
                    posicion++;
                    columna++;
                    break;
                }
            }

            // Si no se reconoce el carácter, marcarlo como desconocido
            if (tokens.Count == 0 || posicion == 0)
            {
                tokens.Add(new Token(TipoToken.Desconocido, caracterActual.ToString(), linea, columna));
                posicion++;
                columna++;
            }
        }

        return tokens;
    }
}
