import {Injectable} from '@angular/core';
import {ExamResult} from '../models/exam-result';
import {HttpClient} from '@angular/common/http';
import {ResourceService} from '@/core/services/resource.service';
import {concatMap, map, Observable} from 'rxjs';
import {StudentSubjectsService} from '@/students/services/student-subjects.service';
import {Exam} from '../models/exam';

@Injectable({
    providedIn: 'root'
})
export class ExamResultsService extends ResourceService<ExamResult> {
    constructor(
        private http: HttpClient,
        private studentSubjectsSvc: StudentSubjectsService
    ) {
        super(http, ExamResult);
    }

    getExamResultsByExamId = (examId: number): Observable<ExamResult[]> => {
        return this.get('/examResults/byExamId/' + examId).pipe(
            map((es) =>
                es.sort((a, b) => a.student?.upi.localeCompare(b.student?.upi))
            )
        );
    };

    loadExamResults = (
        exam: Exam,
        missingMarksReport: boolean
    ): Observable<ExamResult[]> => {
        return this.getExamResultsByExamId(parseInt(exam.id)).pipe(
            concatMap((examResults) =>




                
                this.studentSubjectsSvc
                    .getStudentSubjectsBySchoolClassSubjectId(
                        exam?.schoolClassId,
                        exam?.subjectId
                    )
                    .pipe(
                        map((ss) => {
                            let retRes: ExamResult[] = [];
                            ss.forEach((s) => {
                                let er = new ExamResult();
                                let erResult = examResults.find(
                                    (r) =>
                                        r.examId == parseInt(exam.id) &&
                                        r.studentId == parseInt(s.id)
                                );

                                er.examId = parseInt(exam.id);
                                er.exam = exam;
                                er.studentId = parseInt(s.id);
                                er.score = erResult ? erResult.score : null;
                                er.id = erResult ? erResult.id : null;

                                if (missingMarksReport) {
                                    if (!erResult || !erResult.id) {
                                        retRes.push(er);
                                    }
                                } else {
                                    retRes.push(er);
                                }
                            });
                            return retRes;
                        })
                    )
            )
        );
    };
}
