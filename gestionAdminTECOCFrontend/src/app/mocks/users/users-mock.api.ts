import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { UsersApi } from '../../users/users-api';
import { UserAccount } from '../../users/users.models';

const USERS: UserAccount[] = [
  { id: 'u1', name: 'Camila Restrepo', userName: 'crestrepo', documentType: 'CC', documentNumber: '1094567890', email: 'camila.restrepo@tecoc.edu.co', enabled: true, group: null },
  { id: 'u2', name: 'Julián Torres', userName: 'jtorres', documentType: 'CC', documentNumber: '1098234567', email: 'julian.torres@tecoc.edu.co', enabled: true, group: null },
  { id: 'u3', name: 'Valentina Gómez', userName: 'vgomez', documentType: 'CE', documentNumber: '3452198760', email: 'valentina.gomez@tecoc.edu.co', enabled: false, group: null },
  { id: 'u4', name: 'Andrés Muñoz', userName: 'amunoz', documentType: 'CC', documentNumber: '1102345678', email: 'andres.munoz@tecoc.edu.co', enabled: true, group: null },
  { id: 'u5', name: 'Laura Serna', userName: 'lserna', documentType: 'CC', documentNumber: '1087654321', email: 'laura.serna@tecoc.edu.co', enabled: true, group: null },
  { id: 'u6', name: 'Esteban Cárdenas', userName: 'ecardenas', documentType: 'TI', documentNumber: '1005678912', email: 'esteban.cardenas@tecoc.edu.co', enabled: false, group: null },
  { id: 'u7', name: 'Daniela Peña', userName: 'dpena', documentType: 'CC', documentNumber: '1076543219', email: 'daniela.pena@tecoc.edu.co', enabled: true, group: null },
  { id: 'u8', name: 'Ricardo Bermúdez', userName: 'rbermudez', documentType: 'CC', documentNumber: '1093456782', email: 'ricardo.bermudez@tecoc.edu.co', enabled: true, group: null },
  { id: 'u9', name: 'Sofía Londoño', userName: 'slondono', documentType: 'CE', documentNumber: '3467891205', email: 'sofia.londono@tecoc.edu.co', enabled: false, group: null },
  { id: 'u10', name: 'Mateo Cifuentes', userName: 'mcifuentes', documentType: 'NIT', documentNumber: '900123456', email: 'mateo.cifuentes@tecoc.edu.co', enabled: true, group: null },
];

@Injectable()
export class UsersMockApi extends UsersApi {
  getUsers(): Observable<UserAccount[]> {
    return of(USERS).pipe(delay(300));
  }
}
