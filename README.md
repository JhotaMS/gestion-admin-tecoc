<!-- Provide an overview of what your template package does and how to get started.
Consider previewing the README before uploading (https://learn.microsoft.com/en-us/nuget/nuget-org/package-readme-on-nuget-org#preview-your-readme). -->
# Plantilla Hexagonal Architecture
La arquitectura hexagonal, también conocida como "Arquitectura de Puerto y Adaptador", es un patrón de arquitectura de software que promueve una separación clara entre el núcleo de la aplicación (la lógica de negocio) y las partes externas (interfaces de usuario, bases de datos, servicios externos, etc.). Este patrón fue propuesto por Alistair Cockburn en 2005.

## Principios Clave de la Arquitectura Hexagonal

1. Separación de Responsabilidades: La arquitectura se divide en tres áreas principales: el núcleo de la aplicación, los puertos y los adaptadores.

2. Independencia del Núcleo: El núcleo de la aplicación (dominio) no debe depender de detalles externos. Debe estar aislado y ser independiente, con sus reglas de negocio y lógica encapsulada.

3. Interacción a través de Puertos: Los puertos son interfaces que definen cómo las partes externas pueden interactuar con el núcleo de la aplicación. Actúan como puntos de entrada y salida.

4. Adaptadores para Comunicación: Los adaptadores son implementaciones concretas de los puertos. Permiten que las diferentes tecnologías externas (bases de datos, APIs, interfaces de usuario) interactúen con el núcleo de la aplicación.

## Componentes de la Arquitectura Hexagonal
1. Núcleo de la Aplicación (Dominio):

> - Entidades: Objetos del dominio con identidad propia y ciclo de vida.
> - Agregados: Grupos de entidades relacionadas que se tratan como una sola unidad.
> - Servicios del Dominio: Lógica de negocio que no encaja bien en entidades o agregados.
> - Repositorios: Interfaces para acceder a los objetos del dominio.

2. Puertos:

> - Puertos de Entrada: Interfaces que definen los casos de uso del sistema (cómo puede ser usado).
> - Puertos de Salida: Interfaces que definen las operaciones necesarias en el dominio que dependen de servicios externos.

3. Adaptadores:
> - Adaptadores de Entrada: Implementaciones de puertos de entrada (como controladores web, interfaces de usuario).
> - Adaptadores de Salida: Implementaciones de puertos de salida (como repositorios de bases de datos, clientes de servicios externos).
## Ventajas de la Arquitectura Hexagonal
- Flexibilidad y Mantenibilidad: Facilita el cambio de tecnologías externas sin afectar el núcleo de la aplicación.

- Pruebas y Testeabilidad: El núcleo independiente permite pruebas unitarias y de integración más efectivas.

- Escalabilidad y Extensibilidad: Permite agregar nuevas funcionalidades y tecnologías sin reestructurar el sistema.

- Claridad y Separación de Preocupaciones: Promueve una arquitectura más limpia y entendible.

# Creación de la plantilla

Es importante haber leido los siguientes articulos de microsoft
> https://learn.microsoft.com/es-es/dotnet/core/tutorials/cli-templates-create-item-template
> 
> https://learn.microsoft.com/es-es/dotnet/core/tutorials/cli-templates-create-project-template
>
> https://learn.microsoft.com/es-es/dotnet/core/tutorials/cli-templates-create-template-package?pivots=dotnet-8-0
>

## Empaquetar e instalar
Guarde el archivo de proyecto. Antes de compilar el paquete de plantillas, compruebe que la estructura de carpetas sea correcta. Cualquier plantilla que quiera empaquetar debe colocarse en la carpeta de plantillas, en su propia carpeta. La estructura de carpetas debe un aspecto similar a la jerarquía siguiente:

<pre><code>
working
│   AdatumCorporation.Utility.Templates.csproj
└───content
    └───gestionAdminTECOCApi
        └───.template.config
                template.json
</code></pre>

La carpeta de contenido tiene dos carpetas: extensions y consoleasync.

En el terminal, en la carpeta de trabajo, ejecute el comando <code>dotnet pack AdatumCorporation.Utility.Templates.csproj</code>. Este comando compila el proyecto y crea un paquete de NuGet en la carpeta working\bin\Debug, como se indica en el siguiente resultado:
<pre><code>
MSBuild version 17.8.0-preview-23367-03+0ff2a83e9 for .NET
  Determining projects to restore...
  Restored C:\code\working\AdatumCorporation.Utility.Templates.csproj (in 1.16 sec).

  AdatumCorporation.Utility.Templates -> C:\code\working\bin\Release\net8.0\AdatumCorporation.Utility.Templates.dll
  Successfully created package 'C:\code\working\bin\Release\AdatumCorporation.Utility.Templates.1.0.0.nupkg'.
</code></pre>

A continuación, instale el paquete de plantillas con el comando ``dotnet new install``. En Windows:
<pre><code>
dotnet new install .\bin\Release\AdatumCorporation.Utility.Templates.1.0.0.nupkg
</code></pre>

En Linux o macOS:
<pre><code>
dotnet new install bin/Release/AdatumCorporation.Utility.Templates.1.0.0.nupkg
</code></pre>

## Usar plantilla
Ahora que tiene instalada una plantilla de proyecto, pruébela.

- Crear una carpeta donde desee y pongale el nombre del proyecto
- Ingrese a la carpeta creada y abra una consola.
- Ejecute el siguiente comando ``dotnet new gestionAdminTECOCApi -n Tecoc``. Donde
> - gestionAdminTECOCApi es el nombre de la plantilla
> - Tecoc es el nombre que quiero tener en mi proyecto