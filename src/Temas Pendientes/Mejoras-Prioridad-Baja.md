# Mejoras pendientes · 🟢 Prioridad baja

> 📌 **Qué es este documento**
> Última parte del inventario de mejoras. Aquí están los puntos de **prioridad baja**: limpieza,
> coherencia, mensajes y documentación. Nada de esto rompe nada hoy, pero **todo junto es lo que
> diferencia una biblioteca cuidada de una que parece a medio hacer**.
> La numeración continúa la de [`Mejoras-Prioridad-Media.md`](Mejoras-Prioridad-Media.md), que
> termina en el punto **63**.

> 💡 **Consejo de uso:** muchos de estos puntos se resuelven en bloque con un analizador o un
> `.editorconfig` (ver [`Profesionalizacion-Ingenieria-y-Calidad.md`](Profesionalizacion-Ingenieria-y-Calidad.md)).
> Antes de arreglarlos uno a uno, merece la pena activar las herramientas: el propio compilador irá
> señalando los que quedan.

---

## Índice

- [Código muerto y archivos vacíos](#código-muerto-y-archivos-vacíos) — puntos 64-70
- [Erratas en identificadores públicos](#erratas-en-identificadores-públicos) — puntos 71-75
- [Mensajes al usuario](#mensajes-al-usuario) — puntos 76-79
- [Coherencia y documentación](#coherencia-y-documentación) — puntos 80-87

---

## Código muerto y archivos vacíos

- [ ] **64. `IEFRepoWriterFp` con el cuerpo íntegramente comentado**
    - **Proyecto:** `MoralesLarios.OOFP.EFCore`
    - **Archivo:** `Repos/IEFRepoWriterFp.cs`
    - **Problema:** el archivo existe y la interfaz está declarada, pero todos sus miembros están comentados.
    - **Impacto:** quien lee el proyecto no sabe si la escritura está por implementar o si la interfaz se abandonó.
    - **Propuesta:** completarla como contrato real de escritura o eliminar el archivo (el historial de Git conserva el contenido).

- [ ] **65. `EFCore/Helpers/Constants.cs` es una clase vacía**
    - **Proyecto:** `MoralesLarios.OOFP.EFCore`
    - **Problema:** clase sin ningún miembro.
    - **Impacto:** aparece en IntelliSense y en la documentación generada sin aportar nada.
    - **Propuesta:** eliminarla, o usarla precisamente para centralizar los literales que hoy están repartidos (nombres de cabecera, claves de detalle, tamaños de página por defecto).

- [ ] **66. `Services/GenService.cs` es un archivo vacío**
    - **Proyecto:** `MoralesLarios.OOFP.WebServices`
    - **Problema:** archivo sin contenido efectivo.
    - **Impacto:** sugiere que existe una versión no funcional del servicio junto a la funcional.
    - **Propuesta:** eliminarlo.

- [ ] **67. `RangeEnumValueObject.cs` completamente comentado**
    - **Proyecto:** `MoralesLarios.OOFP.ValueObjects`
    - **Problema:** el tipo entero está comentado, incluida su lógica de validación de rangos de `enum`.
    - **Impacto:** una funcionalidad útil queda invisible; y quien la necesite no sabrá que existía.
    - **Propuesta:** decidir: recuperarlo con pruebas o borrarlo. Si se recupera, aprovechar para basarlo en la factoría única propuesta para los *value objects*.

- [ ] **68. ~20 líneas de código muerto en `SimpleMlComplexPkControllerBase`**
    - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
    - **Archivo / clase:** `SimpleMlComplexPkControllerBase<TEntity, TDto>`
    - **Problema:** bloque de código comentado dentro de la clase.
    - **Impacto:** ruido en cada revisión del archivo y dudas sobre si es la implementación «buena».
    - **Propuesta:** eliminarlo.

- [ ] **69. `#region private methods` vacía**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Archivo / clase:** clases de cliente
    - **Problema:** región declarada sin contenido.
    - **Impacto:** trivial, pero acumulado con los puntos anteriores da la impresión de código sin revisar.
    - **Propuesta:** eliminarla. Considerar prescindir de `#region` en general: si un archivo las necesita para orientarse, probablemente deba dividirse.

- [ ] **70. `ILogger<>` inyectado y nunca utilizado (4 clases)**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Archivo / clase:** `GenClientFp<TDto>`, `GenClientFp<TRequest, TResponse>`, `GenComplexClientFp<TDto>`, `GenComplexClientFp<TRequest, TResponse>`
    - **Problema:** se recibe un `ILogger<>` por constructor y no se emite ni una traza.
    - **Impacto:** dependencia innecesaria y, sobre todo, **oportunidad perdida**: el cliente HTTP es justo el punto donde más falta hace saber qué se envió y qué se recibió.
    - **Propuesta:** usarlo con `[LoggerMessage]` y `EventId` catalogados (petición, respuesta, fallo, reintento), sin volcar cuerpos ni cabeceras sensibles. Si se decide no registrar nada, quitar el parámetro.

---

## Erratas en identificadores públicos

> ⚠️ Todos estos cambios son **rompedores** para quien ya use la biblioteca. La propuesta es la misma
> en todos los casos: **añadir el nombre correcto, marcar el antiguo como `[Obsolete]`** con mensaje
> que indique el sustituto, y retirarlo en la siguiente versión mayor. El detalle está en
> [`Consejos-Nomenclatura.md`](Consejos-Nomenclatura.md).

- [ ] **71. `TryMapIAsyncf`**
    - **Proyecto:** núcleo `MoralesLarios.OOFP`
    - **Archivo:** `Types/MlResultActionsMap.cs` (≈ líneas 483, 498 y 505)
    - **Problema:** la `I` sobrante y la `f` final son claramente un error de teclado, repetido en tres sobrecargas.
    - **Propuesta:** `TryMapAsync`.

- [ ] **72. `ChangeReturnResultAlwais*`**
    - **Proyecto:** núcleo `MoralesLarios.OOFP`
    - **Problema:** «Alwais» por «Always», en toda la familia.
    - **Propuesta:** `ChangeReturnResultAlways*`. Al ser una familia completa, es un buen candidato para hacerlo de una vez en la versión 2.0.

- [ ] **73. `FromStringLenght`, `FromIntLenght`, `MinLenght`**
    - **Proyecto:** `MoralesLarios.OOFP.ValueObjects`
    - **Problema:** «Lenght» por «Length», en varios tipos.
    - **Propuesta:** corregir a `Length`. Es la errata más visible de la biblioteca porque aparece en las factorías que se usan al escribir el primer *value object*.

- [ ] **74. `Id.Bydouble` y `Id.Fromdouble`**
    - **Proyecto:** `MoralesLarios.OOFP.ValueObjects`
    - **Problema:** «double» en minúscula rompe el `PascalCase` que siguen sus hermanos (`ByInt`, `ByString`).
    - **Propuesta:** `ByDouble` y `FromDouble`.

- [ ] **75. `AddScopedtGenServicesFpWithoutReposGeneral` y su variante dúplex**
    - **Proyecto:** `MoralesLarios.OOFP.WebServices`
    - **Archivo / clase:** `RegisterServices`
    - **Problema:** la `t` sobrante en «Scopedt».
    - **Impacto:** es un método de registro, es decir, **una de las primeras cosas que escribe el usuario**; la errata se ve en el primer minuto.
    - **Propuesta:** `AddScopedGenServicesFpWithoutReposGeneral`.

- [ ] **76. `PostGetPaginationAsync<TRequest, TEnumrableResponse>` y «BaseAdress»**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Problema:** «TEnumrable» por «TEnumerable» en un **parámetro genérico** (visible en IntelliSense y en la documentación), y «BaseAdress» por «BaseAddress» repetido unas ocho veces en comentarios públicos.
    - **Propuesta:** corregir ambos. El del parámetro genérico también es rompedor si alguien lo especifica por nombre.

---

## Mensajes al usuario

- [ ] **77. Mensajes en inglés incorrecto en las validaciones**
    - **Proyectos:** `MoralesLarios.OOFP.Validation`, `MoralesLarios.OOFP.Validation.Dataannotations`
    - **Ejemplos:** `"source no be null"`, `"source no be empty"`, `"{x} no be null"`.
    - **Impacto:** son mensajes que **llegan al cliente final**; transmiten descuido y no ayudan a corregir el problema.
    - **Propuesta:** `"{Member} must not be null"`, `"{Member} must not be empty"`, con plantilla y parámetros nombrados, en archivos de recursos.

- [ ] **78. Mensajes en inglés incorrecto en los controladores**
    - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
    - **Ejemplos:** `"isn't null"`, `"is diferent type"`, `"soported"`, y el texto de `GetPkValuesErrorMessage`.
    - **Impacto:** igual que el anterior, con el agravante de que estos aparecen en respuestas HTTP públicas.
    - **Propuesta:** reescribir con mensajes que indiquen el **formato esperado** («expected 2 key values separated by ';', received 3»), y llevarlos a recursos.

- [ ] **79. Mensajes en español codificados en la capa web**
    - **Proyectos:** `MoralesLarios.OOFP.WebApi`, `MoralesLarios.OOFP.WebControllers`
    - **Ejemplos:** el mensaje fijo en español de las extensiones de resultado y el helper `ContieneCombinacion`.
    - **Impacto:** una biblioteca no puede imponer el idioma de las respuestas de la aplicación que la usa; y un identificador en español entre cientos en inglés rompe la coherencia.
    - **Propuesta:** idioma neutro **inglés** por defecto, sustituible por recursos; renombrar el helper a `ContainsCombination` (o eliminarlo, ya que la comparación de textos debe desaparecer con los códigos de error tipados).

- [ ] **80. Mensajes de log y de error genéricos**
    - **Proyecto:** `MoralesLarios.OOFP.WebServices`
    - **Problema:** los textos por defecto no dicen qué entidad, qué clave ni qué operación ha fallado.
    - **Impacto:** un log que dice «error» sin contexto no sirve para diagnosticar en producción.
    - **Propuesta:** plantillas con parámetros nombrados (entidad, operación, clave) y `EventId` estables.

---

## Coherencia y documentación

- [ ] **81. `[Range(0, int.MinValue)]` define un rango imposible**
    - **Proyecto:** `MoralesLarios.OOFP.Internals`
    - **Archivos:** `Info/PaginationInfo.cs`, `Info/PaginationResultInfo.cs`
    - **Problema:** el máximo es `int.MinValue`, menor que el mínimo, así que el rango está vacío.
    - **Impacto:** la validación **no valida nada** (o rechaza todo, según el validador). La paginación queda sin control de límites.
    - **Propuesta:** `[Range(1, int.MaxValue)]` para el número de página y `[Range(1, MaxPageSize)]` para el tamaño, con el máximo tomado de las opciones.

- [ ] **82. `CallRequest*ParamsInfo` sin validación ni contrato claro**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Problema:** `PageNumber` y `PageSize` sin atributos de validación; `Dictionary<string,string>? Headers = null!` (nulable y a la vez `null!`); un operador implícito con tipos que no encajan; los `record` genérico y no genérico sin relación entre ellos; `params object[] pk` colocado tras parámetros opcionales.
    - **Impacto:** el tipo se puede construir en estados sin sentido, y `params` tras opcionales obliga a llamadas incómodas.
    - **Propuesta:** validación en los miembros, `Headers` como `IReadOnlyDictionary<string,string>?` con valor por defecto `null` real, jerarquía común entre las variantes y reordenación de parámetros.

- [ ] **83. Cadenas de log con `<` sin cerrar**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Problema:** mensajes que incluyen nombres genéricos con `<` sin su cierre.
    - **Impacto:** si esos textos acaban en HTML o XML (portales de logs, informes), rompen el formato.
    - **Propuesta:** usar `typeof(T).Name` como parámetro estructurado en lugar de incrustarlo en el texto.

- [ ] **84. `IHttpClientFactoryManager` con nulabilidad inconsistente**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Problema:** unos parámetros usan `= null` y otros `= null!` para el mismo propósito; y el segundo parámetro genérico se llama `K`.
    - **Impacto:** el consumidor no sabe qué puede omitir; y `K` no dice nada (la convención es `TKey`).
    - **Propuesta:** unificar la nulabilidad (declarar `T?` con `= null`) y renombrar `K` a `TKey`.

- [ ] **85. Sobrecarga de `PostGetAsync` comentada en la interfaz**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Problema:** queda una declaración comentada dentro de la interfaz pública.
    - **Impacto:** sugiere una capacidad que no existe.
    - **Propuesta:** eliminarla, o implementarla si sigue teniendo sentido.

- [ ] **86. `MlFile.EndpointPattern` y `MlDirectory.EndpointPattern` mal nombrados**
    - **Proyecto:** `MoralesLarios.OOFP.ValueObjects.IO`
    - **Problema:** `EndpointPattern` sugiere una ruta de API, cuando en realidad describe un patrón de nombre de archivo o carpeta.
    - **Impacto:** el nombre engaña en IntelliSense y en la documentación.
    - **Propuesta:** `NamePattern` o `SearchPattern` (este último es el término que usa la propia BCL en `Directory.GetFiles`).

- [ ] **87. Documentación XML incoherente con el código**
    - **Proyectos:** `MoralesLarios.OOFP.WebControllers` y otros
    - **Ejemplo:** el comentario de `SimpleMlComplexPkControllerBase.DeleteAsync` documenta «200 OK» mientras el código devuelve **204 No Content**.
    - **Impacto:** la documentación errónea es peor que la ausente: el cliente programa contra lo que dice el comentario.
    - **Propuesta:** activar `GenerateDocumentationFile`, revisar los comentarios de los miembros públicos y hacer que los códigos de estado se declaren con `[ProducesResponseType]`, de modo que el compilador y OpenAPI sean la fuente única.

- [ ] **88. `GlobalUsings.cs` con `using` duplicados**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients` (revisar el resto)
    - **Problema:** `MoralesLarios.OOFP.Types` aparece repetido.
    - **Impacto:** advertencia del compilador y confusión sobre qué está disponible globalmente.
    - **Propuesta:** depurar cada `GlobalUsings.cs` y mantener una convención: solo los *namespaces* que se usen en más de la mitad de los archivos.

- [ ] **89. Faltan `README.md` en dos proyectos**
    - **Proyectos:** `MoralesLarios.OOFP.EFCore.WebApi`, `MoralesLarios.OOFP.WebControllers.Cache`
    - **Problema:** son los dos únicos proyectos de la solución sin documentación propia.
    - **Impacto:** no se sabe para qué existen ni cómo se usan; si son experimentos, tampoco se sabe si son publicables.
    - **Propuesta:** añadir un `README.md` con el mismo formato que el resto (propósito, instalación, ejemplo mínimo, registro de servicios), o marcarlos `<IsPackable>false</IsPackable>` si son internos.

---

## Ver también

- [`Mejoras-Prioridad-Critica-y-Alta.md`](Mejoras-Prioridad-Critica-y-Alta.md) — prioridades 🔴 crítica y 🟠 alta.
- [`Mejoras-Prioridad-Media.md`](Mejoras-Prioridad-Media.md) — rendimiento, diseño y API pública.
- [`Profesionalizacion-Ingenieria-y-Calidad.md`](Profesionalizacion-Ingenieria-y-Calidad.md) ·
  [`Profesionalizacion-Diseno-API-y-Producto.md`](Profesionalizacion-Diseno-API-y-Producto.md)
- [`Consejos-Nomenclatura.md`](Consejos-Nomenclatura.md) — propuesta de renombrado.
- [`README.md`](README.md) — índice de la carpeta.
