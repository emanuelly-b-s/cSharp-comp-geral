using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

int _count = 0;
Universidade uni = new Universidade();

Pesquisa("As disciplinas com mais de 10 caractéres no nome.");
Pesquisador.Pesquisa1(uni);
WriteLine();

Pesquisa("Os departamentos, em ordem alfabética, com o número de disciplinas.");
Pesquisador.Pesquisa2(uni);
WriteLine();

Pesquisa("Liste os alunos e suas idades com seus respectivos professores.");
Pesquisador.Pesquisa3(uni);
WriteLine();

Pesquisa("Liste os professores e seus salários com seus respectivos alunos.");
Pesquisador.Pesquisa4(uni);
WriteLine();

Pesquisa("Top 10 Professores com mais alunos da universidade.");
Pesquisador.Pesquisa5(uni);
WriteLine();

Pesquisa("Considerando que todo aluno custa 300 reais mais o salário dos seus professores"
    + " divido entre seus colegas de classe. Liste os alunos e seus respectivos custos.");
Pesquisador.Pesquisa6(uni);
WriteLine();

ReadKey(true);
void Pesquisa(string texto) => WriteLine($"Pesquisa {++_count}. {texto}\n");

public class Pesquisador
{
    /// <summary>
    /// As disciplinas com mais de 10 caractéres no nome
    /// </summary>
    public static void Pesquisa1(Universidade uni)
    {
        foreach (var item in uni.Disciplinas.Where(d => d.Nome.Length > 10))
            WriteLine(item.Nome);
    }

    //&-----------------------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Os departamentos, em ordem alfabética, com o número de disciplinas.
    /// </summary>
    public static void Pesquisa2(Universidade uni)
    {
        var query = uni.Departamentos.Join(uni.Disciplinas,
                                        dep => dep.ID,
                                        disciplina => disciplina.DepartamentoID,
                                        (dep, disciplina) => new
                                        {
                                            NomeDep = dep.Nome,
                                            Disciplinas = disciplina.Nome
                                        })
                                        .GroupBy(dept => dept.NomeDep)
                                        .Select(g => new
                                        {
                                            NomeDep = g.Key,
                                            Disciplinas = g.Count()
                                        });

        foreach (var item in query)
            WriteLine($"Departamento: {item.NomeDep} - Qtd de disciplinas cadastradas: {item.Disciplinas}");

    }

    //&-----------------------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Liste os alunos com seus respectivos professores
    /// </summary>


    public static void Pesquisa3(Universidade uni)
    {
        var queryProf = uni.Turmas.Join(uni.Professores,
                                        profTurma => profTurma.ProfessorID,
                                        idProf => idProf.ID,
                                        (profTurma, idProf) => new
                                        {
                                            idTurma = profTurma.ID,
                                            NomeProf = idProf.Nome
                                        });

        var queryAluno = uni.Alunos.Select(a =>
                                        {
                                            var prof = queryProf
                                                        .Where(pf => a.TurmasMatriculados.Contains(pf.idTurma))
                                                        .DistinctBy(pf => pf.NomeProf);
                                            return new
                                            {
                                                NomeAluno = a.Nome,
                                                IdadeAluno = a.Idade,
                                                Professores = prof
                                            };
                                        });



        foreach (var item in queryAluno)
        {
            WriteLine($"Nome do Aluno: {item.NomeAluno} - Idade {item.IdadeAluno}");
            WriteLine($"---Seus respectivos professores---");
            foreach (var profAluno in item.Professores)
            {
                Write($"{profAluno.NomeProf} ");
            }
            WriteLine("\n");
        }

    }

    //&-----------------------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Liste o número de alunos que cada professor possui.
    /// </summary>
    public static void Pesquisa4(Universidade uni)
    {
        var queryaluno = uni.Turmas.Select(t => new
        {
            ProfessorID = t.ProfessorID,
            QtdAlunos = uni.Alunos.Where(a => a.TurmasMatriculados.Contains(t.ID))
                                  .Count()
        });

        var query = uni.Professores.Join(queryaluno,
                                    prof => prof.ID,
                                    id_prof_da_turma => id_prof_da_turma.ProfessorID,
                                    (professor, turma) => new
                                    {
                                        ProfessorNome = professor.Nome,
                                        SalarioProf = professor.Salario,
                                        QtdAlunos = turma.QtdAlunos
                                    })
                                    .GroupBy(p => p.ProfessorNome)
                                    .Select(g =>
                                    {
                                        return new
                                        {
                                            ProfessorNome = g.Key,
                                            QtdAlunos = g.Sum(p => p.QtdAlunos),
                                            SalarioProf = g.Sum(Sal => Sal.SalarioProf)
                                        };
                                    });


        foreach (var item in query)
            WriteLine($"Professor: {item.ProfessorNome} - Salario {item.SalarioProf} - Qtd Alunos {item.QtdAlunos}");

    }

    //&-----------------------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Top 10 Professores com mais alunos da universidade
    /// </summary>
    public static void Pesquisa5(Universidade uni)
    {
        var queryaluno = uni.Turmas.Select(t => new
        {
            ProfessorID = t.ProfessorID,
            QtdAlunos = uni.Alunos.Where(a => a.TurmasMatriculados.Contains(t.ID))
                                  .Count()
        });

        var query = uni.Professores.Join(queryaluno,
                                    prof => prof.ID,
                                    id_prof_da_turma => id_prof_da_turma.ProfessorID,
                                    (professor, turma) => new
                                    {
                                        ProfessorNome = professor.Nome,
                                        QtdAlunos = turma.QtdAlunos
                                    })
                                    .GroupBy(p => p.ProfessorNome)
                                    .Select(g =>
                                    {
                                        return new
                                        {
                                            ProfessorNome = g.Key,
                                            QtdAlunos = g.Sum(p => p.QtdAlunos),
                                        };
                                    })
                                    .Take(10).OrderByDescending(a => a.QtdAlunos);

        var cont = 1;
        WriteLine("Top 10 Professores com mais Alunos");
        foreach (var item in query)
        {
            WriteLine($"{cont}º: {item.ProfessorNome}  - Qtd Alunos {item.QtdAlunos}");
                        cont++;
        }
            

    }

    /// <summary>
    /// Considerando que todo aluno custa 300 reais mais o salário dos seus professores
    /// divido entre seus colegas de classe. Liste os alunos e seus respectivos custos
    /// </summary>
    public static void Pesquisa6(Universidade uni)
    {
        var queryaluno = uni.Turmas.Select(t => new
        {
            ProfessorID = t.ProfessorID,
            QtdAlunos = uni.Alunos.Where(a => a.TurmasMatriculados.Contains(t.ID))
                                  .Count(),
            Alunos =  uni.Alunos.Where(a => a.TurmasMatriculados.Contains(t.ID))         
        });
        

        var query = uni.Professores.Join(queryaluno,
                                    prof => prof.ID,
                                    id_prof_da_turma => id_prof_da_turma.ProfessorID,
                                    (professor, turma) => new
                                    {
                                        ProfessorNome = professor.Nome,
                                        ProfSalario = professor.Salario,
                                        QtdAlunos = turma.QtdAlunos,
                                        Alunos = turma.Alunos
                                    })
                                    .GroupBy(a => a.Alunos)
                                    .Select(g =>
                                    {
                                        return new
                                        {
                                            NomeAlunos = g.Key,
                                            Custo = g.Sum(p => p.ProfSalario / p.QtdAlunos) + 300,
                                        };
                                    });
        
        foreach (var alunos in query)
        {
            WriteLine($"{alunos.NomeAlunos} - Custo {alunos.Custo}");
        }
    }
}

