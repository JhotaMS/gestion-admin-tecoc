import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { UsersApi } from '../../users/users-api';
import { UserAccount } from '../../users/users.models';

const USERS: UserAccount[] = [
  { id: 'u1', name: 'Camila Restrepo', email: 'camila.restrepo@tecoc.edu.co', role: 'Recursos Humanos', registeredAtIso: '2026-08-12', status: 'activo' },
  { id: 'u2', name: 'Julián Torres', email: 'julian.torres@tecoc.edu.co', role: 'Operaciones', registeredAtIso: '2026-08-10', status: 'activo' },
  { id: 'u3', name: 'Valentina Gómez', email: 'valentina.gomez@tecoc.edu.co', role: 'Finanzas', registeredAtIso: '2026-08-07', status: 'pendiente' },
  { id: 'u4', name: 'Andrés Muñoz', email: 'andres.munoz@tecoc.edu.co', role: 'Sistemas', registeredAtIso: '2026-08-03', status: 'activo' },
  { id: 'u5', name: 'Laura Serna', email: 'laura.serna@tecoc.edu.co', role: 'Atención al cliente', registeredAtIso: '2026-07-29', status: 'activo' },
  { id: 'u6', name: 'Esteban Cárdenas', email: 'esteban.cardenas@tecoc.edu.co', role: 'Ventas', registeredAtIso: '2026-07-22', status: 'pendiente' },
  { id: 'u7', name: 'Daniela Peña', email: 'daniela.pena@tecoc.edu.co', role: 'Marketing', registeredAtIso: '2026-07-15', status: 'activo' },
  { id: 'u8', name: 'Ricardo Bermúdez', email: 'ricardo.bermudez@tecoc.edu.co', role: 'Operaciones', registeredAtIso: '2026-07-09', status: 'activo' },
  { id: 'u9', name: 'Sofía Londoño', email: 'sofia.londono@tecoc.edu.co', role: 'Sistemas', registeredAtIso: '2026-07-02', status: 'pendiente' },
  { id: 'u10', name: 'Mateo Cifuentes', email: 'mateo.cifuentes@tecoc.edu.co', role: 'Finanzas', registeredAtIso: '2026-06-28', status: 'activo' },
];

@Injectable()
export class UsersMockApi extends UsersApi {
  getUsers(): Observable<UserAccount[]> {
    return of(USERS).pipe(delay(300));
  }
}
