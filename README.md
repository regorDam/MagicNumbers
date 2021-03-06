# KC-Games - MagicNumbers

[![KC](https://games.kintoncloud.com/assets/img/PoweredBy.png)](https://kintoncloud.com)

<img align="right" width="300" src="https://user-images.githubusercontent.com/9436924/114465721-c92f4100-9be7-11eb-80f8-c6e66c36a5b7.gif">

Descargar APK:https://cutt.ly/evrFAUl



**Nota:** La aplicación requiere internet.

## Documentación

Este documento pretende explicar un poco los pasos seguidos en el desarrollo del proyecto y explicar el porqué de algunas decisiones tomadas.
Para empezar se han creado algunos scripts con algunas utilidades generales. 
- Se ha creado una clase *Console* para agilizar la activación y desactivación de los Logs en consola. Ya que es altamente recomendado no pintar mensajes por consola al realizar un compilado definitivo o release.
- Un Script MonoBehevior básico (**Init.cs**) con una única función (podría estar integrado en otro script, pero es tan simple que no pone ningún problema...) cuya finalidad es limitar los FPS del juego. Ya que no existe ninguna necesidad de exprimir al hardware al máximo y teniendo en cuenta que los dispositivos móviles se calientan mucho.
- También encontramos un script *FakeProgressbar*, totalmente prescindible, pero para que la aplicación no sea tan "sosa" se ha creado una escena de splash con la simulación de una barra de carga.

Por otro lado, una parte importante es el script APINumbers que contiene dos clases. 
```sh
[Serializable]
public class APINumbers
{
    public string label;
    public int value;
}

[Serializable]
public class NumbersCollection
{
    public APINumbers[] numbers;
}
```

Esto se ha realizado ya que un requisito es que fuera escalable. Creo que la mejor manera de escalar el ejercicio es usar un JSON donde podamos tener la lista de números. Permitiendo añadir más sin necesidad de volver a compilar y permitiendo añadir multiidioma de una forma muy sencilla con un cambio mínimo en el código. Podríamos tener un json en archivo local, pero aun así creo que la mejor opción es obtener a través de una sencilla API. En este caso se ha desarrollado esta API usando Laravel, pero no vamos a entrar más detalle. La URL de destino es: https://games.kintoncloud.com/numbers

Resultado de la petición a la URL:
```sh
{"numbers":[{"label":"cero","value":0},{"label":"uno","value":1},{"label":"dos","value":2},{"label":"tres","value":3},{"label":"cuatro","value":4},{"label":"cinco","value":5},{"label":"sies","value":6},{"label":"siete","value":7},{"label":"ocho","value":8},{"label":"nueve","value":9},{"label":"diez","value":10}]}
```

Para poder trabajar con las clases anteriores se ha creado un controlador **APIController** para permitir lanzar las peticiones GET y su posterior manipulación en una colección del tipo array como puede verse en la clase *NumbersCollection*. Para la obtención de elementos aleatorios en realidad se ha usado una Linked list. Detallaremos la información y su uso más adelante.

A continuación también existen algunos Managers (**LevelManager, GameManager, StateManager**). Estas clases usan el patrón de diseño Singleton básicamente por una cosa muy sencilla. Son clases/objetos de los que no existe ninguna necesidad de tener varias instancias, es más **NO** queremos que existan varias instancias de ellas y queremos que siempre estén disponibles cuando las necesitemos. **GameManager** es el principal y el cual se nutre un poco de los otros. 
- **LevelManager**, es el más simple y tiene como finalidad ayudar a cargar entre la escena de splash a la escena de juego. Es cierto que en este caso con pocas escenas o incluso podríamos solo tener una. Pero es altamente recomendable, ya que si existe la opción de crecimiento al final esta clase puede llegar a ser muy útil.
- **StateManager**, esta clase implica la creación de otros scripts (**ISTate, AState, StatemenentState y AnswerState**). Básicamente se ha decidido dividir la mecánica del juego en una máquina de estados. Estas divisiones están realizadas en dos fases, la fase de enunciado y la fase de respuesta. Permitiendo mantener las dos fases separadas nos aseguramos una correcta interacción entre los botones, haciendo que podamos "despreocuparnos" de cuando serán clicables, si ahora podemos o si ahora no. El loop del juego se basa en pasar de forma infinita de una fase a otra. Para ello se ha creado una interficie IState donde definimos el ciclo de vida de nuestros estados. En este caso no se usan todos ellos, pero es una estructura básica que permitirá añadir nuevos estados con nuevas funcionalidades o finalidades.
```sh
public interface IState 
{
    void Enter();
    void Leave();
    void Pause();
    void Resume();

    void Update(float deltaTime);
    void OnGUI();


    IState GetParent();
}
```
- Luego tenemos una clase abstracta (**AState**), ya que no queremos permitir la creación de instancias directamente de ella. En este caso es muy simple, ya que los requisitos no pedían mucha complicación, es una clase que implementa la interficie anterior con un constructor y algún método genérico que usaremos todos los estados.
```sh
public class AState : IState
{
    protected GameObject go;
    protected string TAG;

    protected GameManager gameManager;

    public AState(string TAG)
    {
        this.TAG = TAG;
        go = GameManager.Instance.FindInScene(TAG);
        go.SetActive(false);
        gameManager = GameManager.Instance;
    }


    virtual public void Enter()
    {
        go.SetActive(true);
    }
    ...
    ...
    ...
    ...
}
```
- Y para terminar con la máquina de estados tenemos la implementación de los dos estados (**StatementState y AnswerState**) las dos clases que contienen la lógica para cada uno de ellos.
El estado StatementState es bastante simple, en cambio el AnswerState es más interesante. Cabe destacar la forma de como pinta las opciones para las respuestas. Para ello se una linked-list porque su rendimiento a la hora de añadir y quitar elementos es muy óptima.
A continuación se muestran algunos fragmentos para llevar a cabo esa tarea.
```sh
...
...
...
numbersList = new LinkedList<APINumbers>();
...
...
...
numbersCollection = gameManager.GetNumbersCollection();
numbersList.Clear();
foreach (APINumbers number in numbersCollection.numbers)
{
     numbersList.AddFirst(number);
}
numbersList.Remove(gameManager.GetCurrentNumber());
...
...
...
public APINumbers GetRandomNumber()
{        
    int l = numbersList.Count();
    int rand = Random.Range(0, l);

    APINumbers n = numbersList.ElementAt<APINumbers>(rand);

    numbersList.Remove(n);
    return n;
}
...
...
...
```



- **Gamenager**, al tener la máquina de estados este script no es muy complejo, cabe destacar como parte importante la inicialización y preparación de los estados

    

```sh
	
stateManager = StateManager.Instance;

stateManager.RegisterState(State.STATEMENT,new StatementState());
stateManager.RegisterState(State.ANSWER, new AnswerState());
        
stateManager.OnStateChange += HandleOnStateChange;

```
Y el método **HandleOnStateChange()** que utilizaremos para reiniciar aquellas variables que debemos reiniciar al pasar de un estado al otro.

## Posibles actualizaciones

- Una actualización interesante podría ser la forma de conectarse a la API, por ejemplo usando LocalStorage para guardar el json y actualizarlo cuando sea posible la conexión internet. De esta forma podríamos usar la aplicación sin necesidad de internet.

- Otra actualización interesante seria añadir el multiidioma, creando nuevas url's en la API para obtener los datos para los diferentes idiomas, en cuanto a codigo C# sería muy fácil añadiendo unas pequeñas flags por ejemplo en GameManager o creando un nuevo script LanguageManager o de ajustes.

- La opción de añadir más números no la contemplamos, ya que la aplicación ya soporta N elementos, simplemente tendríamos que modificar la API para que retorne más elementos.

- Existen mucha/infinitas opciones de mejora o añadir nuevas funcionalidades, pero a priori con los requerimientos actuales estas parecen las más importantes/evidentes.


Code & Love!
