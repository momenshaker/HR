import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { NestedTreeControl } from '@angular/cdk/tree';
import { MatTreeModule, MatTreeNestedDataSource } from '@angular/material/tree';
import { EmployeesApiService, EmployeeHierarchyNode } from './employees.api';

@Component({
  selector: 'app-employee-hierarchy',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTreeModule
  ],
  templateUrl: './employee-hierarchy.component.html',
  styleUrls: ['./employee-hierarchy.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EmployeeHierarchyComponent implements OnInit {
  private readonly api = inject(EmployeesApiService);
  private readonly router = inject(Router);

  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly treeControl = new NestedTreeControl<EmployeeHierarchyNode>((node) => node.directReports ?? []);
  readonly dataSource = new MatTreeNestedDataSource<EmployeeHierarchyNode>();

  ngOnInit(): void {
    this.api.getHierarchy().subscribe({
      next: (hierarchy) => {
        this.dataSource.data = this.normalizeHierarchy(hierarchy);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Unable to load the hierarchy. Please try again later.');
        this.loading.set(false);
      }
    });
  }

  hasChild = (_: number, node: EmployeeHierarchyNode) => !!node.directReports?.length;

  employeeName(node: EmployeeHierarchyNode): string {
    if (!node.employee) {
      return 'Vacant';
    }
    return `${node.employee.firstName} ${node.employee.lastName}`;
  }

  jobTitle(node: EmployeeHierarchyNode): string {
    if (!node.employee) {
      return 'Unfilled role';
    }
    return node.employee.jobTitle || 'Role filled';
  }

  goToEmployees(): void {
    this.router.navigate(['/employees']);
  }

  private normalizeHierarchy(nodes: EmployeeHierarchyNode[] | null | undefined): EmployeeHierarchyNode[] {
    if (!nodes?.length) {
      return [];
    }

    return nodes.map((node) => ({
      ...node,
      directReports: this.normalizeHierarchy(node.directReports)
    }));
  }
}
