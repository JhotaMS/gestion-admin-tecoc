import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { UsersApi } from '../../users/users-api';
import { UserAccount } from '../../users/users.models';

const USERS: UserAccount[] = [
  { id: 'u1', name: 'Camila Restrepo', userName: 'crestrepo', documentType: 'CC', documentNumber: '1094567890', email: 'camila.restrepo@tecoc.edu.co', role: 'Recursos Humanos', registeredAtIso: '2026-08-12', status: 'activo' },
  { id: 'u2', name: 'Julián Torres', userName: 'jtorres', documentType: 'CC', documentNumber: '1098234567', email: 'julian.torres@tecoc.edu.co', role: 'Operaciones', registeredAtIso: '2026-08-10', status: 'activo' },
  { id: 'u3', name: 'Valentina Gómez', userName: 'vgomez', documentType: 'CE', documentNumber: '3452198760', email: 'valentina.gomez@tecoc.edu.co', role: 'Finanzas', registeredAtIso: '2026-08-07', status: 'pendiente' },
  { id: 'u4', name: 'Andrés Muñoz', userName: 'amunoz', documentType: 'CC', documentNumber: '1102345678', email: 'andres.munoz@tecoc.edu.co', role: 'Sistemas', registeredAtIso: '2026-08-03', status: 'activo' },
  { id: 'u5', name: 'Laura Serna', userName: 'lserna', documentType: 'CC', documentNumber: '1087654321', email: 'laura.serna@tecoc.edu.co', role: 'Atención al cliente', registeredAtIso: '2026-07-29', status: 'activo' },
  { id: 'u6', name: 'Esteban Cárdenas', userName: 'ecardenas', documentType: 'TI', documentNumber: '1005678912', email: 'esteban.cardenas@tecoc.edu.co', role: 'Ventas', registeredAtIso: '2026-07-22', status: 'pendiente' },
  { id: 'u7', name: 'Daniela Peña', userName: 'dpena', documentType: 'CC', documentNumber: '1076543219', email: 'daniela.pena@tecoc.edu.co', role: 'Marketing', registeredAtIso: '2026-07-15', status: 'activo' },
  { id: 'u8', name: 'Ricardo Bermúdez', userName: 'rbermudez', documentType: 'CC', documentNumber: '1093456782', email: 'ricardo.bermudez@tecoc.edu.co', role: 'Operaciones', registeredAtIso: '2026-07-09', status: 'activo' },
  { id: 'u9', name: 'Sofía Londoño', userName: 'slondono', documentType: 'CE', documentNumber: '3467891205', email: 'sofia.londono@tecoc.edu.co', role: 'Sistemas', registeredAtIso: '2026-07-02', status: 'pendiente' },
  { id: 'u10', name: 'Mateo Cifuentes', userName: 'mcifuentes', documentType: 'NIT', documentNumber: '900123456', email: 'mateo.cifuentes@tecoc.edu.co', role: 'Finanzas', registeredAtIso: '2026-06-28', status: 'activo' },
];

@Injectable()
export class UsersMockApi extends UsersApi {
  getUsers(): Observable<UserAccount[]> {
    return of(USERS).pipe(delay(300));
  }
}
