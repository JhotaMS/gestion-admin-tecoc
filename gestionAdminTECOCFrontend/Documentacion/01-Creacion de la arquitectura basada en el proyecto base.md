# Migración y refactorización de arquitectura — gestionAdminTECOCFrontend

## Objetivo

Analiza y adapta la arquitectura del proyecto de referencia **Minimal** al proyecto **gestionAdminTECOCFrontend**, manteniendo la funcionalidad, estructura visual y buenas prácticas del proyecto de referencia, pero respetando la arquitectura y convenciones existentes en `gestionAdminTECOCFrontend`.

Antes de realizar cambios, debes **analizar ambos proyectos**, identificar sus diferencias arquitectónicas y proponer la estrategia de migración. No realices cambios destructivos ni reemplaces configuraciones existentes sin justificar previamente el motivo.

---

## 1. Análisis inicial de la arquitectura

Primero realiza una revisión completa de:

* Estructura de carpetas.
* Módulos y componentes.
* Routing.
* Servicios.
* Guards.
* Interceptors.
* Manejo de autenticación.
* Manejo del estado.
* Configuración de ambientes.
* Configuración de estilos.
* Configuración de assets.
* Configuración de Angular.
* Dependencias utilizadas.
* Comunicación con APIs.
* Configuración de Signal/SignalR.
* Configuración de WebSocket.
* Configuración de despliegue.

Analiza especialmente el proyecto **Minimal** como arquitectura de referencia y determina qué elementos deben ser migrados a:

`gestionAdminTECOCFrontend`

### Importante

No copies código de manera indiscriminada.

Para cada elemento que vayas a migrar:

1. Identifica cómo funciona en Minimal.
2. Identifica el equivalente en `gestionAdminTECOCFrontend`.
3. Adapta el código a la arquitectura destino.
4. Conserva las convenciones existentes del proyecto destino cuando sean correctas.
5. Evita duplicar servicios, componentes o configuraciones.
6. No elimines funcionalidad existente sin una justificación técnica.

---

# 2. Migración de la arquitectura de Minimal

Utiliza el proyecto **Minimal** como referencia principal y adapta su arquitectura al proyecto:

`gestionAdminTECOCFrontend`

La migración debe incluir, cuando corresponda:

* Estructura de carpetas.
* Componentes.
* Layout.
* Navegación.
* Routing.
* Servicios.
* Guards.
* Interceptors.
* Modelos/interfaces.
* Configuración de autenticación.
* Configuración de estilos.
* Assets.
* Configuración visual.
* Manejo de errores.
* Estructura base de las páginas.

Crea en `gestionAdminTECOCFrontend` una estructura de carpetas equivalente a la utilizada por Minimal, pero adaptada a las necesidades y convenciones del proyecto destino.

---

# 3. Migración visual

Replica la experiencia visual del proyecto Minimal en `gestionAdminTECOCFrontend`.

Debes copiar/adaptar:

* Todos los estilos necesarios.
* Variables CSS/SCSS.
* Tipografías.
* Themes.
* Colores.
* Espaciados.
* Botones.
* Formularios.
* Cards.
* Tablas.
* Iconografía.
* Layout.
* Responsive design.
* Estados hover/focus/active.
* Componentes reutilizables.

El resultado debe ser visualmente consistente con Minimal.

No dupliques estilos innecesariamente. Si existen estilos equivalentes en el proyecto destino, reutilízalos o refactorízalos.

---

# 4. Crear Sidebar vertical

Implementa en `gestionAdminTECOCFrontend` un **Sidebar vertical** basado en el Sidebar utilizado por Minimal.

Debe contemplar:

* Estructura visual equivalente.
* Menú principal.
* Submenús.
* Iconos.
* Estados activo/inactivo.
* Navegación mediante Angular Router.
* Responsive design.
* Comportamiento colapsable, si existe en Minimal.
* Integración con el layout principal.

El Sidebar debe estar preparado para crecer posteriormente con nuevos módulos y opciones de navegación.

---

# 5. Crear Dashboard

Crea el primer componente funcional:

`Dashboard`

Utiliza como referencia:

`minimal/src/app/starter/starter.component.ts`

Analiza primero cómo está implementado `starter.component.ts` y adapta su estructura al proyecto:

`gestionAdminTECOCFrontend`

El Dashboard debe:

* Mantener la estructura visual de Minimal.
* Utilizar los componentes y estilos migrados.
* Integrarse correctamente con el routing.
* Integrarse con el Sidebar.
* Ser responsive.
* Utilizar datos simulados inicialmente.
* Evitar llamadas reales a APIs mientras se desarrolla esta primera versión.

Si `starter.component.ts` utiliza servicios, modelos o componentes auxiliares, identifica cuáles son necesarios y adáptalos correctamente en lugar de copiar referencias rotas.

---

# 6. Crear Login

Implementa el Login de `gestionAdminTECOCFrontend` utilizando como referencia:

`minimal/src/app/account/auth/signin/cover`

Replica y adapta:

* Diseño.
* Layout.
* Formulario.
* Validaciones.
* Mensajes de error.
* Estados de loading.
* Mostrar/ocultar contraseña.
* Responsive design.
* Estilos.
* Componentes auxiliares.

El Login debe integrarse correctamente con:

* Routing.
* AuthGuard.
* Servicio de autenticación.
* Manejo del token.
* Interceptor HTTP.

Inicialmente, la autenticación debe funcionar mediante **datos simulados**, sin depender de un backend real.

---

# 7. Comunicación mediante SignalR / WebSocket

Analiza detalladamente la configuración actual de Signal/SignalR del proyecto.

El objetivo es garantizar una comunicación estable mediante **WebSocket**, teniendo en cuenta que la aplicación está desplegada en:

**Azure Kubernetes Service (AKS)**

y que el **Ingress del contenedor ya está configurado correctamente**.

### Analiza como mínimo:

* Configuración actual de SignalR.
* `HubConnection`.
* Transportes utilizados.
* Negociación (`negotiate`).
* Configuración de WebSocket.
* Reconnection.
* Manejo de errores.
* Eventos de conexión.
* Eventos de desconexión.
* Keep Alive.
* Server Timeout.
* Access Token.
* Headers.
* CORS.
* Proxy/Ingress.
* Balanceo de carga.
* Sticky Sessions, si son necesarias.
* Configuración relacionada con Azure AKS.

### Objetivo técnico

Refactoriza la configuración para que la aplicación utilice correctamente:

**Angular → Ingress → AKS → Backend/SignalR Hub**

y garantice la comunicación mediante WebSocket cuando el entorno lo permita.

No asumas que el WebSocket funciona únicamente porque el Ingress está configurado.

Verifica también que:

* El cliente solicite correctamente el transporte WebSocket.
* SignalR pueda completar el handshake.
* La conexión pueda recuperarse después de una desconexión.
* Los tokens se gestionen correctamente.
* Existan logs suficientes para diagnosticar problemas.
* No se generen múltiples conexiones innecesarias.
* La conexión se cierre correctamente al destruir el componente/servicio.
* La configuración funcione tanto localmente como en AKS.

Si SignalR necesita fallback a otros transportes por razones de compatibilidad, documenta claramente cuándo ocurre y por qué.

---

# 8. Simulación de APIs

Mientras el backend real no esté disponible, implementa una capa de **APIs simuladas (Mock APIs)**.

No coloques datos simulados directamente dentro de los componentes.

Utiliza una arquitectura que permita posteriormente reemplazar fácilmente los mocks por APIs reales.

Por ejemplo:

```text
Component
   ↓
Service
   ↓
API/Repository
   ↓
Mock API
```

La implementación debe permitir cambiar posteriormente:

```text
Mock API
```

por:

```text
Backend API
```

sin tener que modificar los componentes.

Simula inicialmente los endpoints necesarios para:

* Login.
* Usuario autenticado.
* Perfil.
* Menú.
* Dashboard.
* Datos estadísticos.
* Notificaciones.
* Datos necesarios para demostrar la comunicación SignalR/WebSocket.

Utiliza respuestas tipadas mediante interfaces/modelos.

---

# 9. Arquitectura esperada

Como resultado, busca una arquitectura similar a:

```text
gestionAdminTECOCFrontend
│
├── src
│   ├── app
│   │   ├── core
│   │   │   ├── auth
│   │   │   ├── guards
│   │   │   ├── interceptors
│   │   │   ├── services
│   │   │   └── websocket
│   │   │
│   │   ├── shared
│   │   │   ├── components
│   │   │   ├── models
│   │   │   └── services
│   │   │
│   │   ├── layout
│   │   │   ├── sidebar
│   │   │   ├── header
│   │   │   └── layout
│   │   │
│   │   ├── account
│   │   │   └── auth
│   │   │       └── signin
│   │   │
│   │   ├── dashboard
│   │   │   ├── components
│   │   │   ├── models
│   │   │   └── dashboard.component
│   │   │
│   │   ├── mocks
│   │   │   ├── auth
│   │   │   ├── dashboard
│   │   │   └── notifications
│   │   │
│   │   └── app.routes
│   │
│   ├── assets
│   └── styles
```

La estructura anterior es una referencia. **Adáptala a la arquitectura real de ambos proyectos después de analizarlos.**

---

# 10. Reglas de implementación

Durante la implementación:

* No rompas funcionalidades existentes.
* No elimines archivos sin verificar sus dependencias.
* No dupliques servicios existentes.
* No dupliques estilos si pueden reutilizarse.
* Utiliza TypeScript fuertemente tipado.
* Evita `any` salvo que sea estrictamente necesario.
* Mantén separación de responsabilidades.
* Utiliza componentes reutilizables.
* Utiliza servicios para lógica de negocio.
* Mantén los componentes enfocados en presentación.
* Utiliza interfaces/modelos para los contratos de datos.
* Mantén una configuración clara para environments.
* No hardcodees URLs de APIs.
* No hardcodees tokens.
* Maneja correctamente errores HTTP.
* Maneja estados de loading.
* Agrega logs útiles para diagnosticar WebSocket/SignalR.
* Mantén compatibilidad con el despliegue en Azure AKS.

---

# 11. Estrategia de ejecución

No intentes realizar toda la migración de una sola vez.

Ejecuta el trabajo en las siguientes fases:

### Fase 1 — Análisis

Analiza Minimal y `gestionAdminTECOCFrontend`.

Entrega:

* Arquitectura actual de ambos proyectos.
* Diferencias encontradas.
* Componentes que deben migrarse.
* Dependencias necesarias.
* Riesgos.
* Estrategia de migración.

### Fase 2 — Arquitectura base

Implementa:

* Estructura de carpetas.
* Layout.
* Sidebar.
* Routing.
* Estilos globales.

### Fase 3 — Autenticación

Implementa:

* Login.
* AuthService.
* AuthGuard.
* Interceptor.
* Mock API de autenticación.

### Fase 4 — Dashboard

Implementa el Dashboard basado en:

`minimal/src/app/starter/starter.component.ts`

### Fase 5 — SignalR/WebSocket

Refactoriza y configura la comunicación WebSocket/SignalR.

Incluye:

* Conexión.
* Reconexión.
* Manejo de errores.
* Autenticación.
* Logs.
* Integración con AKS/Ingress.

### Fase 6 — Mock APIs

Implementa los servicios y respuestas simuladas necesarias.

### Fase 7 — Validación

Verifica:

* `npm install`
* `npm run build`
* Tests existentes.
* Routing.
* Login.
* Sidebar.
* Dashboard.
* Mock APIs.
* SignalR.
* WebSocket.
* Reconexión.
* Desconexión.
* Manejo de errores.

---

# 12. Resultado esperado

Al finalizar debes entregar:

1. La arquitectura adaptada de Minimal a `gestionAdminTECOCFrontend`.
2. Los estilos migrados.
3. El Sidebar vertical funcionando.
4. El Login funcionando con Mock API.
5. El Dashboard funcionando.
6. La estructura de carpetas organizada.
7. La capa de servicios preparada para APIs reales.
8. La configuración SignalR/WebSocket refactorizada.
9. La configuración preparada para Azure AKS + Ingress.
10. Manejo de reconexión y errores.
11. Mock APIs funcionales.
12. Documentación breve de los cambios realizados.
13. Lista de archivos creados, modificados y eliminados.
14. Instrucciones para ejecutar el proyecto localmente.
15. Instrucciones para validar la conexión WebSocket en AKS.

### Regla importante

Antes de modificar cualquier archivo, **inspecciona y comprende su contenido y sus dependencias**.

Si encuentras diferencias entre Minimal y `gestionAdminTECOCFrontend`, no copies ciegamente el código. Explica la diferencia y adapta la implementación al proyecto destino.

El objetivo no es simplemente copiar Minimal, sino **reutilizar su arquitectura y experiencia visual para construir una versión correctamente integrada, mantenible y preparada para producción de `gestionAdminTECOCFrontend`.**
