import { Observable } from 'rxjs';
import { ProgramaAcademico } from './programas-academicos.models';

export abstract class ProgramasAcademicosApi {
  abstract getProgramasAcademicos(): Observable<ProgramaAcademico[]>;
}
