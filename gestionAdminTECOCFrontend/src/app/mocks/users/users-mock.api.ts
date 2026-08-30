import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { UsersApi } from '../../users/users-api';
import { UserAccount } from '../../users/users.models';

const USERS: UserAccount[] = [
  { id: 'u1', name: 'Camila Restrepo', userName: 'crestrepo', documentType: 'Cédula de ciudadanía', documentNumber: '1094567890', email: 'camila.restrepo@tecoc.edu.co' },
  { id: 'u2', name: 'Julián Torres', userName: 'jtorres', documentType: 'Cédula de ciudadanía', documentNumber: '1098234567', email: 'julian.torres@tecoc.edu.co' },
  { id: 'u3', name: 'Valentina Gómez', userName: 'vgomez', documentType: 'Cédula de extranjería', documentNumber: '3452198760', email: 'valentina.gomez@tecoc.edu.co' },
  { id: 'u4', name: 'Andrés Muñoz', userName: 'amunoz', documentType: 'Cédula de ciudadanía', documentNumber: '1102345678', email: 'andres.munoz@tecoc.edu.co' },
  { id: 'u5', name: 'Laura Serna', userName: 'lserna', documentType: 'Cédula de ciudadanía', documentNumber: '1087654321', email: 'laura.serna@tecoc.edu.co' },
  { id: 'u6', name: 'Esteban Cárdenas', userName: 'ecardenas', documentType: 'Tarjeta de identidad', documentNumber: '1005678912', email: 'esteban.cardenas@tecoc.edu.co' },
  { id: 'u7', name: 'Daniela Peña', userName: 'dpena', documentType: 'Cédula de ciudadanía', documentNumber: '1076543219', email: 'daniela.pena@tecoc.edu.co' },
  { id: 'u8', name: 'Ricardo Bermúdez', userName: 'rbermudez', documentType: 'Cédula de ciudadanía', documentNumber: '1093456782', email: 'ricardo.bermudez@tecoc.edu.co' },
  { id: 'u9', name: 'Sofía Londoño', userName: 'slondono', documentType: 'Cédula de extranjería', documentNumber: '3467891205', email: 'sofia.londono@tecoc.edu.co' },
  { id: 'u10', name: 'Mateo Cifuentes', userName: 'mcifuentes', documentType: 'Número de identificación tributaria', documentNumber: '900123456', email: 'mateo.cifuentes@tecoc.edu.co' },
];

@Injectable()
export class UsersMockApi extends UsersApi {
  getUsers(): Observable<UserAccount[]> {
    return of(USERS).pipe(delay(300));
  }
}
