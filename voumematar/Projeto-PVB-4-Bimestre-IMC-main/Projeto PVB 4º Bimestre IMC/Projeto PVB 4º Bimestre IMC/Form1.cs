/* *******************************************************************
* Colegio Técnico Antônio Teixeira Fernandes (Univap)
* Curso Técnico em Informática - Data de Entrega: 04/11/2024
* Autores do Projeto: Lukas Macacari de Matos
*                     Lucas Aguilar da Silva Nascimento
* Turma: 2F
* Projeto do 4º Bimestre
* Observação: <colocar se houver>
* 
* problema_aula.cs
* ************************************************************/
using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace Projeto_PVB_4º_Bimestre_IMC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CarregarPacientes(); // Carrega a lista ao iniciar o programa
        }

        int contador_id = 0;

        private void Form1_Load(object sender, EventArgs e) // Executado ao iniciar o programa
        {
            AtualizarContadorId(); // Atualiza o próximo ID com base na quantidade de pacientes no JSON
            CarregarPacientes();
        }

        private void AtualizarContadorId()
        {
            string caminho = @"C:\IMC\IMC.json";

            if (File.Exists(caminho))
            {
                string conteudoJson = File.ReadAllText(caminho).Trim();

                // Verifica se o JSON está vazio ou só possui colchetes
                if (conteudoJson.Length > 2)
                {
                    conteudoJson = conteudoJson.TrimStart('[').TrimEnd(']');
                    var pacientesJson = conteudoJson.Split(new string[] { "},{" }, StringSplitOptions.None);
                    contador_id = pacientesJson.Length + 1; // Define o próximo ID como o número de pacientes + 1
                }
                else
                {
                    contador_id = 1; // Se o arquivo estiver vazio, começa do ID 1
                }
            }
            else
            {
                contador_id = 1; // Se o arquivo não existir, começa do ID 1
            }
        }

        private void SalvarDadosJson(string nome, double peso, double altura, double IMC, string classificacao)
        {
            int ID_paciente = contador_id;

            string novoPacienteJson = "{\n" +
                                  $"  \"Id\": {ID_paciente},\n" +
                                  $"  \"Nome\": \"{nome}\",\n" +
                                  $"  \"Peso (Kg)\": {peso:F2},\n" +
                                  $"  \"Altura (m)\": {altura:F2},\n" +
                                  $"  \"IMC\": {IMC:F2},\n" +
                                  $"  \"Classificação\": \"{classificacao}\"\n" +
                                  "}";

            string caminho = @"C:\IMC\IMC.json";

            if (File.Exists(caminho) && new FileInfo(caminho).Length > 0)
            {
                string conteudoAtual = File.ReadAllText(caminho);
                conteudoAtual = conteudoAtual.TrimEnd('\n', '\r', ' ', ']');
                string novoConteudo = conteudoAtual + ",\n" + novoPacienteJson + "\n]";
                File.WriteAllText(caminho, novoConteudo);
            }
            else
            {
                string novoConteudo = "[\n" + novoPacienteJson + "\n]";
                File.WriteAllText(caminho, novoConteudo);
            }

            contador_id++; // Incrementa o ID para o próximo paciente
            CarregarPacientes(); // Atualiza a lista após salvar um novo paciente
        }

        private void CarregarPacientes()
        {
            listBox1.Items.Clear();
            string caminho = @"C:\IMC\IMC.json";

            if (File.Exists(caminho))
            {
                string[] linhas = File.ReadAllLines(caminho);

                for (int i = 0; i < linhas.Length; i++)
                {
                    if (linhas[i].Contains("\"Id\":"))
                    {
                        string id = linhas[i].Split(':')[1].Trim().TrimEnd(',');
                        string nome = "";
                        string peso = "";

                        if (i + 1 < linhas.Length && linhas[i + 1].Contains("\"Nome\":"))
                        {
                            nome = linhas[i + 1].Split(':')[1].Trim().Trim('"', ',');

                            for (int j = i + 2; j < linhas.Length; j++)
                            {
                                if (linhas[j].Contains("\"Peso (Kg)\":"))
                                {
                                    peso = linhas[j].Split(':')[1].Trim().TrimEnd(',');
                                    break;
                                }
                            }

                            listBox1.Items.Add($"ID: {id} - {nome} - {peso} kg");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Arquivo JSON não encontrado.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dados_paciente(); // Coleta os dados e exibe no formulário
        }

        private void dados_paciente()
        {
            string nome = textBox1.Text;
            textBox6.Text = nome;

            double peso = double.Parse(textBox2.Text);
            double altura = double.Parse(textBox3.Text);
            double IMC = peso / Math.Pow(altura, 2);
            string classificacao = "";

            textBox4.Text = IMC.ToString("F2");

            if (IMC < 18.5) classificacao = "Magreza";
            else if (IMC < 25) classificacao = "Saudável";
            else if (IMC < 30) classificacao = "Sobrepeso";
            else if (IMC < 35) classificacao = "Obesidade Grau I";
            else if (IMC < 40) classificacao = "Obesidade Grau II";
            else classificacao = "Obesidade Grau III";

            textBox5.Text = classificacao;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string nome = textBox1.Text;
            double peso = double.Parse(textBox2.Text);
            double altura = double.Parse(textBox3.Text);
            double IMC = double.Parse(textBox4.Text);
            string classificacao = textBox5.Text;

            SalvarDadosJson(nome, peso, altura, IMC, classificacao);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int id_crud = int.Parse(textBox8.Text);
            string caminho = @"C:\IMC\IMC.json";

            if (File.Exists(caminho))
            {
                string conteudoJson = File.ReadAllText(caminho);
                conteudoJson = conteudoJson.Trim().TrimStart('[').TrimEnd(']');
                var pacientesJson = conteudoJson.Split(new string[] { "},{" }, StringSplitOptions.None);

                List<string> pacientesAtualizados = new List<string>();
                bool pacienteEncontrado = false;

                foreach (var pacienteJson in pacientesJson)
                {
                    string pacienteData = "{" + pacienteJson.Trim('{', '}') + "}";
                    string idString = pacienteData.Split(new string[] { "\"Id\": " }, StringSplitOptions.None)[1].Split(',')[0];
                    int idPaciente = int.Parse(idString.Trim());

                    if (idPaciente == id_crud)
                    {
                        pacienteEncontrado = true;
                        continue;
                    }

                    pacientesAtualizados.Add(pacienteData);
                }

                if (pacienteEncontrado)
                {
                    string jsonAtualizado = "[" + string.Join(",", pacientesAtualizados) + "]";
                    File.WriteAllText(caminho, jsonAtualizado);
                    MessageBox.Show("Paciente excluído com sucesso.");
                }
                else
                {
                    MessageBox.Show("Paciente com o ID especificado não encontrado.");
                }
            }
            else
            {
                MessageBox.Show("Arquivo JSON não encontrado.");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Evento de clique no label1 (vazio)
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Evento de mudança de texto no textBox1 (vazio)
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Evento de mudança de texto no textBox2 (vazio)
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            // Evento de mudança de texto no textBox3 (vazio)
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            // Evento de mudança de texto no textBox4 (vazio)
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            // Evento de mudança de texto no textBox5 (vazio)
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            // Evento de mudança de texto no textBox6 (vazio)
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            // Evento de mudança de texto no textBox8 (vazio)
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Evento de seleção de item na listBox1 (vazio)
        }
    }
}






