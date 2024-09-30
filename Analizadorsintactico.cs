using System;
using System.Collections.Generic;
using System.Collections.Generic;

public class Nodo
{
    public string Valor { get; set; }          // Valor del nodo
    public List<Nodo> Hijos { get; set; }      // Lista de nodos hijos

    public Nodo(string valor)
    {
        Valor = valor;                         // Inicializa el valor del nodo
        Hijos = new List<Nodo>();              // Inicializa la lista de hijos
    }

    public void AgregarHijo(Nodo hijo)
    {
        Hijos.Add(hijo);                       // Agrega un hijo a la lista
    }

    public void Imprimir(string prefijo = "", bool esUltimo = true) // Método para imprimir el árbol
    {
        Console.WriteLine(prefijo + (esUltimo ? "└── " : "├── ") + Valor); // Imprime el valor con prefijo

        prefijo += esUltimo ? "    " : "│   "; // Ajusta el prefijo para los hijos

        for (int i = 0; i < Hijos.Count; i++)
        {
            Hijos[i].Imprimir(prefijo, i == Hijos.Count - 1); // Llama recursivamente a imprimir los hijos
        }
    }
}


public class AnalizadorSintactico
{
    private List<Token> tokens;  // Lista de tokens a analizar
    private int posicion;         // Posición actual en la lista de tokens

    public AnalizadorSintactico(List<Token> tokens)
    {
        this.tokens = tokens;    // Inicializa la lista de tokens
        this.posicion = 0;       // Comienza en la posición 0
    }

    public Nodo Analizar() // Método para iniciar el análisis
    {
        Nodo raiz = new Nodo("Programa"); // Crea el nodo raíz

        while (posicion < tokens.Count) // Mientras queden tokens
        {
            raiz.AgregarHijo(Sentencia()); // Agrega sentencias al árbol
        }
        return raiz; // Devuelve el árbol sintáctico
    }

    private Nodo Sentencia() // Método para analizar una sentencia
    {
        if (tokens[posicion].Tipo == TipoToken.PalabraClave && tokens[posicion].Valor == "si")
        {
            return AnalizarSentenciaIf();
        }
        else if (tokens[posicion].Tipo == TipoToken.PalabraClave && tokens[posicion].Valor == "retorno")
        {
            return AnalizarSentenciaReturn();
        }
        else
        {
            throw new Exception("Sentencia no válida");
        }
    }

    private Nodo AnalizarSentenciaIf() // Método para analizar la sentencia if
    {
        Nodo nodoIf = new Nodo("Sentencia If");
        posicion++; // Consumir "si"
        nodoIf.AgregarHijo(AceptarDelimitador("("));
        nodoIf.AgregarHijo(Expresion()); // Agregar expresión
        nodoIf.AgregarHijo(AceptarDelimitador(")"));
        nodoIf.AgregarHijo(AceptarDelimitador("{"));
        while (posicion < tokens.Count && tokens[posicion].Valor != "}")
        {
            nodoIf.AgregarHijo(Sentencia()); // Agrega sentencias dentro del bloque
        }
        nodoIf.AgregarHijo(AceptarDelimitador("}"));
        return nodoIf; // Devuelve el nodo "Sentencia If"
    }

    private Nodo AnalizarSentenciaReturn() // Método para analizar la sentencia return
    {
        Nodo nodoReturn = new Nodo("Sentencia Return");
        posicion++; // Consumir "retorno"
        nodoReturn.AgregarHijo(AceptarIdentificador()); // Agregar identificador
        nodoReturn.AgregarHijo(AceptarOperadorM()); // Agregar operador
        nodoReturn.AgregarHijo(AceptarNumero()); // Agregar número
        AceptarDelimitador(";"); // Consumir el punto y coma
        return nodoReturn; // Devuelve el nodo "Sentencia Return"
    }

    private Nodo Expresion() // Método para analizar expresiones
    {
        Nodo nodoExp = new Nodo("Expresión");
        nodoExp.AgregarHijo(AceptarIdentificador()); // Agrega identificador
        nodoExp.AgregarHijo(AceptarOperadorL()); // Agrega operador lógico
        nodoExp.AgregarHijo(AceptarNumero()); // Agrega número
        return nodoExp; // Devuelve el nodo de la expresión
    }

    private Nodo AceptarDelimitador(string delimitador)
    {
        if (posicion < tokens.Count && tokens[posicion].Tipo == TipoToken.Delimitador && tokens[posicion].Valor == delimitador)
        {
            Nodo nodoDelimitador = new Nodo(delimitador);
            posicion++;
            return nodoDelimitador;
        }
        else
        {
            throw new Exception($"Se esperaba '{delimitador}'");
        }
    }

    private Nodo AceptarIdentificador()
    {
        if (posicion < tokens.Count && tokens[posicion].Tipo == TipoToken.Identificador)
        {
            Nodo nodoIdentificador = new Nodo(tokens[posicion].Valor);
            posicion++;
            return nodoIdentificador;
        }
        else
        {
            throw new Exception("Se esperaba un identificador");
        }
    }

    private Nodo AceptarOperadorM()
    {
        if (posicion < tokens.Count && tokens[posicion].Tipo == TipoToken.OperadorM)
        {
            Nodo nodoOperador = new Nodo(tokens[posicion].Valor);
            posicion++;
            return nodoOperador;
        }
        else
        {
            throw new Exception("Se esperaba un operador matemático");
        }
    }

    private Nodo AceptarOperadorL()
    {
        if (posicion < tokens.Count && tokens[posicion].Tipo == TipoToken.OperadorL)
        {
            Nodo nodoOperador = new Nodo(tokens[posicion].Valor);
            posicion++;
            return nodoOperador;
        }
        else
        {
            throw new Exception("Se esperaba un operador lógico");
        }
    }

    private Nodo AceptarNumero()
    {
        if (posicion < tokens.Count && tokens[posicion].Tipo == TipoToken.Numero)
        {
            Nodo nodoNumero = new Nodo(tokens[posicion].Valor);
            posicion++;
            return nodoNumero;
        }
        else
        {
            throw new Exception("Se esperaba un número");
        }
    }
}
