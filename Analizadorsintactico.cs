public class AnalizadorSintactico
{
    private List<Token> tokens;
    private int posicion;

    public AnalizadorSintactico(List<Token> tokens)
    {
        this.tokens = tokens;
        this.posicion = 0;
    }

    // Método principal que inicia el análisis sintáctico
    public Nodo Analizar()
    {
        Nodo raiz = new Nodo("Programa");

        // Verificar que el primer token sea "Programa"
        Token primerToken = ObtenerTokenActual();
        if (primerToken.Tipo != TipoToken.PalabraClave || primerToken.Valor != "Programa")
        {
            throw new Exception("Se esperaba la palabra clave 'Programa' al inicio del código.");
        }

        AvanzarToken(); // Saltar "Programa"

        // Analizar el bloque de código después de "Programa"
        VerificarToken(TipoToken.Delimitador, "{");
        AvanzarToken(); // Saltar "{"

        Nodo bloque = AnalizarBloque();
        raiz.AgregarHijo(bloque);

        return raiz;
    }

    // Método para analizar sentencias (if, while, return, declaración)
    private Nodo AnalizarSentencia()
    {
        Token tokenActual = ObtenerTokenActual();

        if (tokenActual.Tipo == TipoToken.PalabraClave)
        {
            switch (tokenActual.Valor)
            {
                case "si":
                    return AnalizarIf();
                case "mientras":
                    return AnalizarWhile();
                case "retorno":
                    return AnalizarReturn();
                default:
                    throw new Exception($"Palabra clave no reconocida: {tokenActual.Valor}");
            }
        }
        else if (tokenActual.Tipo == TipoToken.Identificador)
        {
            return AnalizarDeclaracion();
        }

        throw new Exception($"Sentencia no válida: {tokenActual.Valor}");
    }

    // Análisis de sentencias "if" con "else" opcional
    private Nodo AnalizarIf()
    {
        Nodo nodoIf = new Nodo("Sentencia If");

        // Se espera "si"
        AvanzarToken(); // Saltar "si"

        // Se espera "("
        VerificarToken(TipoToken.Delimitador, "(");
        AvanzarToken(); // Saltar "("

        // Analizar condición
        Nodo condicion = AnalizarExpresion();
        nodoIf.AgregarHijo(condicion);

        // Se espera ")"
        VerificarToken(TipoToken.Delimitador, ")");
        AvanzarToken(); // Saltar ")"

        // Se espera "{"
        VerificarToken(TipoToken.Delimitador, "{");
        AvanzarToken(); // Saltar "{"

        // Analizar sentencias dentro del bloque if
        Nodo bloqueIf = AnalizarBloque();

        if (posicion < tokens.Count && ObtenerTokenActual().Valor == "sino")
        {
            Nodo nodoElse = new Nodo("Sentencia else");
            nodoElse.AgregarHijo(bloqueIf);
            AvanzarToken(); 

            VerificarToken(TipoToken.Delimitador, "{");
            AvanzarToken(); 

            Nodo bloqueElse = AnalizarBloque();
            nodoElse.AgregarHijo(bloqueElse);
            nodoIf.AgregarHijo(nodoElse);
        }
        else
        {
            nodoIf.AgregarHijo(bloqueIf);
        }

        return nodoIf;
    }

    // Análisis de ciclos "while"
    private Nodo AnalizarWhile()
    {
        Nodo nodoWhile = new Nodo("Sentencia While");

        // Se espera "mientras"
        AvanzarToken(); // Saltar "mientras"

        // Se espera "("
        VerificarToken(TipoToken.Delimitador, "(");
        AvanzarToken(); // Saltar "("

        // Analizar condición
        Nodo condicion = AnalizarExpresion();
        nodoWhile.AgregarHijo(condicion);

        // Se espera ")"
        VerificarToken(TipoToken.Delimitador, ")");
        AvanzarToken(); // Saltar ")"

        // Se espera "{"
        VerificarToken(TipoToken.Delimitador, "{");
        AvanzarToken(); // Saltar "{"

        // Analizar sentencias dentro del bloque
        Nodo bloque = AnalizarBloque();
        nodoWhile.AgregarHijo(bloque);

        return nodoWhile;
    }

    // Análisis de sentencias "return"
    private Nodo AnalizarReturn()
    {
        Nodo nodoReturn = new Nodo("Sentencia Return");

        // Se espera "retorno"
        AvanzarToken(); // Saltar "retorno"

        // Analizar expresión de retorno
        Nodo expresion = AnalizarExpresion();
        nodoReturn.AgregarHijo(expresion);

        // Se espera ";"
        VerificarToken(TipoToken.Delimitador, ";");
        AvanzarToken(); // Saltar ";"

        return nodoReturn;
    }

    // Análisis de declaraciones (identificador = expresión;)
    private Nodo AnalizarDeclaracion()
    {
        Nodo nodoDeclaracion = new Nodo("Operador: =");

        // Se espera un identificador
        Token tokenIdentificador = ObtenerTokenActual();
        VerificarToken(TipoToken.Identificador);
        nodoDeclaracion.AgregarHijo(new Nodo($"Identificador: {tokenIdentificador.Valor}"));
        AvanzarToken(); // Saltar el identificador

        // Se espera "="
        VerificarToken(TipoToken.OperadorM, "=");
        AvanzarToken(); // Saltar "="

        // Analizar expresión
        Nodo expresion = AnalizarExpresion();
        nodoDeclaracion.AgregarHijo(expresion);

        // Se espera ";"
        VerificarToken(TipoToken.Delimitador, ";");
        AvanzarToken(); // Saltar ";"

        return nodoDeclaracion;
    }

    // Método para analizar expresiones (ej. x + 1, i <= 100)
    private Nodo AnalizarExpresion()
    {
        Nodo nodoExpresion = null;

        // Primer operando
        Token operando = ObtenerTokenActual();
        if (operando.Tipo == TipoToken.Identificador || operando.Tipo == TipoToken.Numero)
        {
            nodoExpresion = new Nodo("Operador: (temp)"); // Nodo temporal para el operador
            nodoExpresion.AgregarHijo(new Nodo($"Operando: {operando.Valor}"));
            AvanzarToken();
        }
        else
        {
            throw new Exception("Se esperaba un operando");
        }

        // Operador (se puede hacer más general para incluir operadores lógicos)
        if (EsOperador(ObtenerTokenActual()) || EsOperadorComparacion(ObtenerTokenActual()))
        {
            Token operador = ObtenerTokenActual();
            nodoExpresion.Valor = $"Operador: {operador.Valor}"; // Cambia el valor del nodo temporal
            AvanzarToken(); // Saltar el operador

            // Segundo operando
            operando = ObtenerTokenActual();
            if (operando.Tipo == TipoToken.Identificador || operando.Tipo == TipoToken.Numero)
            {
                nodoExpresion.AgregarHijo(new Nodo($"Operando: {operando.Valor}"));
                AvanzarToken();
            }
            else
            {
                throw new Exception("Se esperaba un segundo operando");
            }
        }

        return nodoExpresion;
    }

    private bool EsOperadorComparacion(Token token)
    {
        return token.Tipo == TipoToken.OperadorL && 
            (token.Valor == "<=" || token.Valor == ">=" || token.Valor == "==" || token.Valor == "!=" || token.Valor == "<" || token.Valor == ">" );
    }

    private Nodo AnalizarBloque()
    {
        Nodo nodoBloque = new Nodo("Bloque");

        while (ObtenerTokenActual().Valor != "}")
        {
            Nodo sentencia = AnalizarSentencia();
            if (sentencia != null)
            {
                nodoBloque.AgregarHijo(sentencia);
            }
        }

        // Se espera "}"
        VerificarToken(TipoToken.Delimitador, "}");
        AvanzarToken(); // Saltar "}"

        return nodoBloque;
    }

    private void VerificarToken(TipoToken tipoEsperado, string valorEsperado = null)
    {
        Token tokenActual = ObtenerTokenActual();
        if (tokenActual.Tipo != tipoEsperado || (valorEsperado != null && tokenActual.Valor != valorEsperado))
        {
            throw new Exception($"Error de análisis sintáctico: Se esperaba {tipoEsperado} '{valorEsperado}' y se encontró '{tokenActual.Valor}'");
        }
    }

    private void AvanzarToken()
    {
        posicion++;
    }

    private Token ObtenerTokenActual()
    {
        while (posicion < tokens.Count && tokens[posicion].Tipo == TipoToken.Comentario)
        {
            posicion++; // Ignorar comentarios
        }
        if (posicion >= tokens.Count) throw new Exception("No hay más tokens disponibles");
        return tokens[posicion];
    }

    private bool EsOperador(Token token)
    {
        return token.Tipo == TipoToken.OperadorM || token.Tipo == TipoToken.OperadorL;
    }
}
