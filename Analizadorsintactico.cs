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
                case "para":
                return AnalizarFor();
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
        nodoDeclaracion.AgregarHijo(new Nodo($"Operando: {tokenIdentificador.Valor}"));
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

    // Método para analizar expresiones 
   private Nodo AnalizarExpresion()
{
    // Crear nodo directamente con el operador, si existe.
    Nodo nodoExpresion;

    // Primer operando
    Token operando1 = ObtenerTokenActual();
    if (operando1.Tipo == TipoToken.Identificador || operando1.Tipo == TipoToken.Numero)
    {
        AvanzarToken();
    }
    else
    {
        throw new Exception("Se esperaba un operando");
    }

    // Verificar si hay un operador (puede ser una operación aritmética o de comparación)
    if (EsOperador(ObtenerTokenActual()) || EsOperadorComparacion(ObtenerTokenActual()))
    {
        // Crear nodo con el operador como raíz
        Token operador = ObtenerTokenActual();
        nodoExpresion = new Nodo($"Operador: {operador.Valor}");
        AvanzarToken(); // Saltar el operador

        // Agregar primer operando como hijo
        nodoExpresion.AgregarHijo(new Nodo($"Operando: {operando1.Valor}"));

        // Segundo operando
        Token operando2 = ObtenerTokenActual();
        if (operando2.Tipo == TipoToken.Identificador || operando2.Tipo == TipoToken.Numero)
        {
            nodoExpresion.AgregarHijo(new Nodo($"Operando: {operando2.Valor}"));
            AvanzarToken();
        }
        else
        {
            throw new Exception("Se esperaba un segundo operando");
        }
    }
    else
    {
        // Si no hay operador, simplemente devolver el primer operando como nodo
        nodoExpresion = new Nodo($"Operando: {operando1.Valor}");
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
  private Nodo AnalizarFor()
{
    Nodo nodoFor = new Nodo("Sentencia For");

    // Se espera "para"
    AvanzarToken(); // Saltar "para"

    // Se espera "("
    VerificarToken(TipoToken.Delimitador, "(");
    AvanzarToken(); // Saltar "("

    // Analizar inicialización
    Nodo inicializacion = AnalizarInicializacionFor();
    
    // Se espera ";"
    VerificarToken(TipoToken.Delimitador, ";");
    AvanzarToken(); // Saltar ";"

    // Analizar condición
    Nodo condicion = AnalizarCondicionFor();

    // Se espera ";"
    VerificarToken(TipoToken.Delimitador, ";");
    AvanzarToken(); // Saltar ";"

    // Analizar actualización
    Nodo actualizacion = AnalizarActualizacionFor();

    // Se espera ")"
    VerificarToken(TipoToken.Delimitador, ")");
    AvanzarToken(); // Saltar ")"

    // Crear nodos binarios para condición y actualización
    Nodo condicionYactualizacion = new Nodo("CondicionYActualizacion");
    condicionYactualizacion.AgregarHijo(condicion);
    condicionYactualizacion.AgregarHijo(actualizacion);

    // Se espera "{"
    VerificarToken(TipoToken.Delimitador, "{");
    AvanzarToken(); // Saltar "{"

    // Analizar el bloque de sentencias
    Nodo bloque = AnalizarBloque();

    // Crear nodos binarios para inicialización y el resto del bucle
    Nodo inicializacionYresto = new Nodo("InicializacionYresto");
    inicializacionYresto.AgregarHijo(inicializacion);
    inicializacionYresto.AgregarHijo(condicionYactualizacion);
    
    nodoFor.AgregarHijo(inicializacionYresto);
    nodoFor.AgregarHijo(bloque);

    return nodoFor;
}
private Nodo AnalizarInicializacionFor()
{
    Nodo nodoInicializacion = new Nodo("Inicializacion");

    Token tokenActual = ObtenerTokenActual();
    if (tokenActual.Tipo == TipoToken.Identificador)
    {
        Nodo identificador = new Nodo($"Identificador: {tokenActual.Valor}");
        AvanzarToken(); // Saltar el identificador

        VerificarToken(TipoToken.OperadorM, "=");
        Nodo operador = new Nodo("Operador: =");
        AvanzarToken(); // Saltar "="

        Nodo expresionInicializacion = AnalizarExpresion();

        operador.AgregarHijo(identificador);
        operador.AgregarHijo(expresionInicializacion);

        nodoInicializacion.AgregarHijo(operador);
    }
    else
    {
        throw new Exception("Se esperaba una declaración o asignación en la inicialización del bucle for");
    }

    return nodoInicializacion;
}

private Nodo AnalizarCondicionFor()
{
    Nodo nodoCondicion = new Nodo("Condicion");

    Nodo condicion = AnalizarExpresion();
    nodoCondicion.AgregarHijo(condicion);

    return nodoCondicion;
}

private Nodo AnalizarActualizacionFor()
{
    Nodo nodoActualizacion = new Nodo("Actualizacion");

    Token tokenActual = ObtenerTokenActual();
    if (tokenActual.Tipo == TipoToken.Identificador)
    {
        Nodo identificador = new Nodo($"Identificador: {tokenActual.Valor}");
        AvanzarToken(); // Saltar el identificador

        VerificarToken(TipoToken.OperadorM, "=");
        Nodo operador = new Nodo("Operador: =");
        AvanzarToken(); // Saltar "="

        Nodo expresionActualizacion = AnalizarExpresion();

        operador.AgregarHijo(identificador);
        operador.AgregarHijo(expresionActualizacion);

        nodoActualizacion.AgregarHijo(operador);
    }
    else
    {
        throw new Exception("Se esperaba una asignación en la actualización del bucle for");
    }

    return nodoActualizacion;
}



}
