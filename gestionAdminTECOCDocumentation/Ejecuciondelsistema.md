
Objetivos (Problemas a Resolver)
Problemática Actual (Situación Inicial):
 * Gestión Académica Ineficiente: El uso de planillas en Excel para el registro de notas y asistencia genera fragmentación de la información, riesgo de pérdida de datos, errores en la formulación manual de promedios y demoras en la publicación de calificaciones para los estudiantes.
 * Control Manual de Inventario: El registro de préstamos de equipos mediante papel y firmas físicas dificulta el seguimiento en tiempo real del estado de los activos, genera descontrol en las fechas de devolución, impide detectar reincidencias en mora y carece de auditoría sobre el estado físico de los dispositivos.
Objetivos del Sistema:
 * Centralizar y Automatizar: Sustituir las hojas de cálculo por un módulo digital con cálculo ponderado automático de notas y registro de asistencia con marca de tiempo.
 * Trazabilidad de Activos: Digitalizar el flujo de solicitud, despachos y devoluciones de equipos informáticos, eliminando las planillas de papel y aplicando bloqueos automáticos a usuarios con entregas pendientes.
 * Transparencia en Tiempo Real: Ofrecer a estudiantes y directivos visibilidad inmediata sobre el rendimiento académico, faltas acumuladas y disponibilidad de recursos de laboratorio


1. Actores del Sistema (Stakeholders)
​Docente: Registra la asistencia en aula, parametriza los porcentajes de evaluación, ingresa las calificaciones individuales y solicita equipos para el desarrollo de sus asignaturas.
​Estudiante: Consulta en tiempo real sus notas con promedios recalculados al instante, revisa sus minutos de retardo acumulados y solicita el préstamo de equipos tecnológicos.
​Encargado de Laboratorio / Inventario: Despacha y recibe los equipos, inspecciona su estado físico y gestiona el catálogo de activos disponibles.
​Coordinador Académico: Audita el ingreso oportuno de notas, analiza reportes de deserción/inasistencia y monitorea el uso de los recursos tecnológicos de la facultad.
​2. Modelo de Arquitectura Hexagonal
​La lógica de negocio se aísla completamente en el Dominio, asegurando que los cálculos matemáticos de notas, las validaciones de retardo y las restricciones de préstamos sean independientes de la base de datos o el frontend.