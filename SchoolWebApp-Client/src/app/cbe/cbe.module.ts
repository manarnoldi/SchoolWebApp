import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CompetenciesComponent } from './assessments/components/competencies/competencies.component';
import { AssessmentTypesComponent } from './assessments/components/assessment-types/assessment-types.component';
import { SharedModule } from '@/shared/shared.module';
import { CbeComponent } from './cbe.component';
import { GeneralOutcomesComponent } from './assessments/components/general-outcomes/general-outcomes.component';
import { GeneralOutcomeAddFormComponent } from './assessments/components/general-outcomes/general-outcome-add-form/general-outcome-add-form.component';
import { StrandsComponent } from './assessments/components/strands/strands.component';
import { StrandAddFormComponent } from './assessments/components/strands/strand-add-form/strand-add-form.component';
import { ThemesComponent } from './assessments/components/themes/themes.component';
import { ThemeAddFormComponent } from './assessments/components/themes/theme-add-form/theme-add-form.component';
import { ValuesComponent } from './values/components/values/values.component';
import { ValueScoresComponent } from './values/components/value-scores/value-scores.component';
import { ResponsibilitiesComponent } from './responsibilities/components/responsibilities/responsibilities.component';
import { ActivitiesComponent } from './cocurriculum/components/activities/activities.component';
import { ScoreTypesComponent } from './cocurriculum/components/score-types/score-types.component';
import { CommunityServiceActivitiesComponent } from './community-service/components/activities/activities.component';
import { SubStrandsComponent } from './assessments/components/sub-strands/sub-strands.component';
import { SubStrandAddFormComponent } from './assessments/components/sub-strands/sub-strand-add-form/sub-strand-add-form.component';
import { BroadOutcomesComponent } from './assessments/components/broad-outcomes/broad-outcomes.component';
import { BroadOutcomeAddFormComponent } from './assessments/components/broad-outcomes/broad-outcome-add-form/broad-outcome-add-form.component';
import { SpecificOutcomesComponent } from './assessments/components/specific-outcomes/specific-outcomes.component';
import { SpecificOutcomeAddFormComponent } from './assessments/components/specific-outcomes/specific-outcome-add-form/specific-outcome-add-form.component';
import { StudentAssessmentsComponent } from './assessments/components/student-assessments/student-assessments.component';
import { StudentAssessmentAddFormComponent } from './assessments/components/student-assessments/student-assessment-add-form/student-assessment-add-form.component';
import { ExamTypesComponent } from './exams/components/exam-types/exam-types.component';
import { ExamsComponent } from './exams/components/exams/exams.component';
import { ExamResultsComponent } from './exams/components/exam-results/exam-results.component';
import { ExamResultsClasswiseComponent } from './exams/components/exam-results-classwise/exam-results-classwise.component';
import { ExamResultsBulkComponent } from './exams/components/exam-results-bulk/exam-results-bulk.component';
import { StudentResponsibilitiesComponent } from './responsibilities/components/student-responsibilities/student-responsibilities.component';
import { StudentValueScoresComponent } from './values/components/student-value-scores/student-value-scores.component';
import { CommunityServiceStudentAssignmentsComponent } from './community-service/components/student-assignments/student-assignments.component';
import { ScoresSetupComponent } from './cocurriculum/components/scores-setup/scores-setup.component';
import { StudentCoCurriculumAssignmentsComponent } from './cocurriculum/components/student-assignments/student-assignments.component';
import { StudentCoCurriculumScoresComponent } from './cocurriculum/components/student-scores/student-scores.component';
import { KeyQuestionsComponent } from './assessments/components/key-questions/key-questions.component';
import { KeyQuestionAddFormComponent } from './assessments/components/key-questions/key-question-add-form/key-question-add-form.component';
import { LearningExperiencesComponent } from './assessments/components/learning-experiences/learning-experiences.component';
import { LearningExperienceAddFormComponent } from './assessments/components/learning-experiences/learning-experience-add-form/learning-experience-add-form.component';
import { LessonAllocationsComponent } from './assessments/components/lesson-allocations/lesson-allocations.component';
import { LessonAllocationAddFormComponent } from './assessments/components/lesson-allocations/lesson-allocation-add-form/lesson-allocation-add-form.component';
import { PCIsComponent } from './assessments/components/pcis/pcis.component';
import { PCIAddFormComponent } from './assessments/components/pcis/pci-add-form/pci-add-form.component';

@NgModule({
  declarations: [
    CbeComponent,
    CompetenciesComponent,
    AssessmentTypesComponent,
    GeneralOutcomesComponent,
    GeneralOutcomeAddFormComponent,
    StrandAddFormComponent,
    StrandsComponent,
    ThemesComponent,
    ThemeAddFormComponent,
    ValuesComponent,
    ValueScoresComponent,
    ResponsibilitiesComponent,
    ActivitiesComponent,
    ScoreTypesComponent,
    CommunityServiceActivitiesComponent,
    SubStrandsComponent,
    SubStrandAddFormComponent,
    BroadOutcomesComponent,
    BroadOutcomeAddFormComponent,
    SpecificOutcomesComponent,
    SpecificOutcomeAddFormComponent,
    StudentAssessmentsComponent,
    StudentAssessmentAddFormComponent,
    ExamTypesComponent,
    ExamsComponent,
    ExamResultsComponent,
    ExamResultsClasswiseComponent,
    ExamResultsBulkComponent,
    StudentResponsibilitiesComponent,
    StudentValueScoresComponent,
    CommunityServiceStudentAssignmentsComponent,
    ScoresSetupComponent,
    StudentCoCurriculumAssignmentsComponent,
    StudentCoCurriculumScoresComponent,
    KeyQuestionsComponent,
    KeyQuestionAddFormComponent,
    LearningExperiencesComponent,
    LearningExperienceAddFormComponent,
    LessonAllocationsComponent,
    LessonAllocationAddFormComponent,
    PCIsComponent,
    PCIAddFormComponent
  ],
  imports: [
    CommonModule, SharedModule
  ]
})
export class CbeModule { }
