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
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

namespace Projeto_PVB_4º_Bimestre_IMC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CarregarPacientes(); // Carrega a lista ao iniciar o programa
        }

        // Definição da classe Paciente
        public class Paciente
        {
            public int Id { get; set; }
            public string Nome { get; set; }

            [JsonPropertyName("Peso (Kg)")]
            public double Peso { get; set; }

            [JsonPropertyName("Altura (m)")]
            public double Altura { get; set; }

            public double IMC { get; set; }

            [JsonPropertyName("Classificação")]
            public string Classificacao { get; set; }
        }

        int contador_id = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            AtualizarContadorId(); // Define o próximo ID inicial com base no JSON
            CarregarPacientes(); // Carrega a lista de pacientes na listBox1
        }

        private bool ValidarNome(string nome)
        {
            // Verifica se o nome contém apenas letras e espaços
            return System.Text.RegularExpressions.Regex.IsMatch(nome, @"^[A-Za-z\s]+$");
        }

        private bool ValidarPesoAltura(string valor)
        {
            // Verifica se é um número válido e maior que zero
            return double.TryParse(valor, out double resultado) && resultado > 0;
        }

        private void AtualizarContadorId()
        {
            string caminho = @"C:\IMC\IMC.json";

            if (File.Exists(caminho))
            {
                string conteudoJson = File.ReadAllText(caminho).Trim();

                if (conteudoJson.Length > 2)
                {
                    // Usa System.Text.Json para desserializar e encontrar o maior ID
                    List<Paciente> pacientes = JsonSerializer.Deserialize<List<Paciente>>(conteudoJson);

                    // Obtém o maior ID já registrado
                    int maxId = 0;
                    foreach (var paciente in pacientes)
                    {
                        if (paciente.Id > maxId)
                        {
                            maxId = paciente.Id;
                        }
                    }

                    // Define o próximo ID como o maior ID encontrado + 1
                    contador_id = maxId + 1;
                }
                else
                {
                    contador_id = 1; // JSON está vazio, então começa do ID 1
                }
            }
            else
            {
                contador_id = 1; // Arquivo não existe, começa do ID 1
            }
        }

        // Função para obter o menor ID disponível
        private int ObterProximoIdDisponivel(List<Paciente> pacientes)
        {
            int idDisponivel = 1;

            // Ordena os pacientes por ID e verifica onde há lacunas
            var idsExistentes = pacientes.Select(p => p.Id).OrderBy(id => id).ToList();

            // Encontra o primeiro ID ausente
            foreach (var id in idsExistentes)
            {
                if (id == idDisponivel)
                {
                    idDisponivel++;
                }
                else
                {
                    break;
                }
            }

            return idDisponivel;
        }

        private void SalvarDadosJson(string nome, double peso, double altura, double IMC, string classificacao)
        {
            string caminho = @"C:\IMC\IMC.json";
            List<Paciente> pacientes;

            // Carrega o conteúdo atual do JSON, se existir
            if (File.Exists(caminho) && new FileInfo(caminho).Length > 0)
            {
                string conteudoAtual = File.ReadAllText(caminho);
                pacientes = JsonSerializer.Deserialize<List<Paciente>>(conteudoAtual);
            }
            else
            {
                pacientes = new List<Paciente>();
            }

            // Define o próximo ID disponível para o novo paciente
            int ID_paciente = ObterProximoIdDisponivel(pacientes);

            Paciente novoPaciente = new Paciente
            {
                Id = ID_paciente,
                Nome = nome,
                Peso = peso,
                Altura = altura,
                IMC = IMC,
                Classificacao = classificacao
            };

            // Adiciona o novo paciente à lista
            pacientes.Add(novoPaciente);

            // Serializa a lista completa de pacientes para o JSON
            string novoConteudo = JsonSerializer.Serialize(pacientes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(caminho, novoConteudo);

            CarregarPacientes(); // Atualiza a listBox1 com a lista de pacientes
        }


        private void CarregarPacientes()
        {
            listBox1.Items.Clear();
            string caminho = @"C:\IMC\IMC.json";

            if (File.Exists(caminho))
            {
                string conteudoJson = File.ReadAllText(caminho).Trim();

                // Verifica se o conteúdo JSON está vazio ou apenas contém colchetes vazios
                if (string.IsNullOrEmpty(conteudoJson) || conteudoJson == "[]")
                {
                    MessageBox.Show("O arquivo JSON está vazio.");
                    return;
                }

                // Tenta desserializar e trata qualquer exceção que possa ocorrer
                try
                {
                    List<Paciente> pacientes = JsonSerializer.Deserialize<List<Paciente>>(conteudoJson);
                    foreach (var paciente in pacientes)
                    {
                        listBox1.Items.Add($"ID: {paciente.Id} - {paciente.Nome} - {paciente.Peso:F2} kg");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar pacientes: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Arquivo JSON não encontrado.");
            }
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

        private Dictionary<string, int> ContarClassificacoes()
        {
            string caminho = @"C:\IMC\IMC.json";
            var contagem = new Dictionary<string, int>
    {
        { "Magreza", 0 },
        { "Saudável", 0 },
        { "Sobrepeso", 0 },
        { "Obesidade Grau I", 0 },
        { "Obesidade Grau II", 0 },
        { "Obesidade Grau III", 0 }
    };

            if (File.Exists(caminho))
            {
                string conteudoJson = File.ReadAllText(caminho);
                List<Paciente> pacientes = JsonSerializer.Deserialize<List<Paciente>>(conteudoJson);

                foreach (var paciente in pacientes)
                {
                    if (contagem.ContainsKey(paciente.Classificacao))
                    {
                        contagem[paciente.Classificacao]++;
                    }
                }
            }

            return contagem;
        }


        private void GerarRelatorioHtml(Dictionary<string, int> contagemClassificacoes)
        {
            try
            {
                string caminhoHtml = @"C:\IMC\relatorio_imc.html";

                // Carrega os dados dos pacientes
                string caminhoJson = @"C:\IMC\IMC.json";
                List<Paciente> pacientes = new List<Paciente>();
                if (File.Exists(caminhoJson))
                {
                    string conteudoJson = File.ReadAllText(caminhoJson);
                    pacientes = JsonSerializer.Deserialize<List<Paciente>>(conteudoJson);
                }

                // Construção da tabela HTML com os dados dos pacientes
                string tabelaHtml = "<table style='width:100%; border-collapse: collapse;'>";
                tabelaHtml += "<tr><th>Nome</th><th>Peso (kg)</th><th>Altura (m)</th><th>IMC</th><th>Classificação</th></tr>";

                foreach (var paciente in pacientes)
                {
                    tabelaHtml += $"<tr>" +
                                  $"<td>{paciente.Nome}</td>" +
                                  $"<td>{paciente.Peso}</td>" +
                                  $"<td>{paciente.Altura}</td>" +
                                  $"<td>{paciente.IMC:F2}</td>" +
                                  $"<td>{paciente.Classificacao}</td>" +
                                  $"</tr>";
                }

                tabelaHtml += "</table>";

                // Construção do gráfico e do layout HTML completo
                string html = $@"
<!DOCTYPE html>
<html lang='pt-BR'>
<head>
<meta charset='UTF-8'>
<title>Relatório de IMC</title>
<script src='https://cdn.plot.ly/plotly-2.27.0.min.js'></script>
<style>
    table {{
        width: 100%;
        border-collapse: collapse;
        font-family: Arial, sans-serif;
    }}
    th, td {{
        padding: 12px;
        text-align: left;
        border: 1px solid #ddd;
    }}
    th {{
        background-color: #f2f2f2;
    }}
</style>
</head>
<body>
<center><div id='myDiv' style='width: 500px; height:500px;'>Pesquisa de Índice de Massa Corporal</div></center>
<br>
<center>{tabelaHtml}</center>
<script>
var data = [{{
    type: 'scatterpolar',
    r: [{contagemClassificacoes["Magreza"]}, {contagemClassificacoes["Saudável"]}, {contagemClassificacoes["Sobrepeso"]}, {contagemClassificacoes["Obesidade Grau I"]}, {contagemClassificacoes["Obesidade Grau II"]}, {contagemClassificacoes["Obesidade Grau III"]}],
    theta: ['Magreza', 'Saudável', 'Sobrepeso', 'Obesidade Grau I', 'Obesidade Grau II', 'Obesidade Grau III'],
    fill: 'toself'
}}];
var layout = {{
    polar: {{
        radialaxis: {{
            visible: true,
            range: [0, {Math.Max(1, contagemClassificacoes.Values.Max())}]
        }}
    }},
    showlegend: false
}};
Plotly.newPlot('myDiv', data, layout);
</script>
</body>
</html>";

                // Escreve o conteúdo HTML no arquivo
                File.WriteAllText(caminhoHtml, html);

                // Abre o arquivo HTML no navegador
                Process.Start(new ProcessStartInfo(caminhoHtml) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar o relatório HTML: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //BUTTONS do 1 ao 7 para daqui pra baixo!!!!     


        private void button1_Click(object sender, EventArgs e)
        {
            string nome = textBox1.Text;

            // Verifica se o nome é válido
            if (!ValidarNome(nome))
            {
                MessageBox.Show("O nome deve conter apenas letras e espaços.", "Erro de Validação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Valida o campo de peso
            if (!ValidarPesoAltura(textBox2.Text))
            {
                MessageBox.Show("O peso deve ser um número positivo.", "Erro de Validação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Valida o campo de altura
            if (!ValidarPesoAltura(textBox3.Text))
            {
                MessageBox.Show("A altura deve ser um número positivo.", "Erro de Validação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            dados_paciente(); // Coleta os dados e exibe no formulário
        }


        private void button2_Click(object sender, EventArgs e)
        {
            // Verifica se o campo de ID está preenchido
            if (int.TryParse(textBox8.Text, out int id_crud))
            {
                string caminho = @"C:\IMC\IMC.json";

                if (File.Exists(caminho))
                {
                    string conteudoJson = File.ReadAllText(caminho);
                    List<Paciente> pacientes = JsonSerializer.Deserialize<List<Paciente>>(conteudoJson);

                    // Encontra o paciente pelo ID
                    Paciente pacienteEncontrado = pacientes.Find(p => p.Id == id_crud);
                    if (pacienteEncontrado != null)
                    {
                        // Remove o paciente da lista
                        pacientes.Remove(pacienteEncontrado);

                        // Salva a lista atualizada de pacientes no arquivo JSON
                        string jsonAtualizado = JsonSerializer.Serialize(pacientes, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(caminho, jsonAtualizado);

                        MessageBox.Show("Paciente excluído com sucesso.");
                        CarregarPacientes(); // Atualiza a listBox1 com a lista de pacientes atualizada
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
            else
            {
                MessageBox.Show("Por favor, insira um ID válido.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Verifica se todos os campos estão preenchidos
            if (string.IsNullOrWhiteSpace(textBox8.Text) ||
                string.IsNullOrWhiteSpace(textBox9.Text) ||
                string.IsNullOrWhiteSpace(textBox10.Text) ||
                string.IsNullOrWhiteSpace(textBox11.Text))
            {
                MessageBox.Show("Por favor, preencha todos os campos obrigatórios (ID, Nome, Peso, Altura).", "Campos obrigatórios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tenta converter o valor de ID para o tipo correto
            if (!int.TryParse(textBox8.Text, out int id))
            {
                MessageBox.Show("Por favor, insira um valor válido para ID.", "Entrada inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Valida o nome
            string nome = textBox9.Text;
            if (!ValidarNome(nome))
            {
                MessageBox.Show("O nome deve conter apenas letras e espaços.", "Erro de Validação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Valida o campo de peso
            if (!ValidarPesoAltura(textBox10.Text))
            {
                MessageBox.Show("O peso deve ser um número positivo.", "Erro de Validação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Valida o campo de altura
            if (!ValidarPesoAltura(textBox11.Text))
            {
                MessageBox.Show("A altura deve ser um número positivo.", "Erro de Validação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Converte os valores de peso e altura
            double peso = double.Parse(textBox10.Text);
            double altura = double.Parse(textBox11.Text);

            // Define o caminho para o arquivo JSON
            string caminho = @"C:\IMC\IMC.json";

            // Verifica se o arquivo JSON existe
            if (File.Exists(caminho))
            {
                // Carrega o conteúdo do arquivo JSON
                string conteudoJson = File.ReadAllText(caminho);
                List<Paciente> pacientes = JsonSerializer.Deserialize<List<Paciente>>(conteudoJson);

                // Encontra o paciente pelo ID
                Paciente pacienteEncontrado = pacientes.FirstOrDefault(p => p.Id == id);
                if (pacienteEncontrado != null)
                {
                    // Atualiza os dados do paciente
                    pacienteEncontrado.Nome = nome;
                    pacienteEncontrado.Peso = peso;
                    pacienteEncontrado.Altura = altura;
                    pacienteEncontrado.IMC = Math.Round(peso / Math.Pow(altura, 2), 2); // Arredonda IMC para 2 casas decimais

                    // Define a classificação do paciente atualizado com base no IMC
                    if (pacienteEncontrado.IMC < 18.5) pacienteEncontrado.Classificacao = "Magreza";
                    else if (pacienteEncontrado.IMC < 25) pacienteEncontrado.Classificacao = "Saudável";
                    else if (pacienteEncontrado.IMC < 30) pacienteEncontrado.Classificacao = "Sobrepeso";
                    else if (pacienteEncontrado.IMC < 35) pacienteEncontrado.Classificacao = "Obesidade Grau I";
                    else if (pacienteEncontrado.IMC < 40) pacienteEncontrado.Classificacao = "Obesidade Grau II";
                    else pacienteEncontrado.Classificacao = "Obesidade Grau III";

                    // Serializa a lista completa de pacientes para o JSON
                    string jsonAtualizado = JsonSerializer.Serialize(pacientes, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(caminho, jsonAtualizado);

                    MessageBox.Show("Dados do paciente atualizados com sucesso.");
                    CarregarPacientes(); // Atualiza a listBox1 com a lista de pacientes atualizada
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


        private void button4_Click(object sender, EventArgs e)
        {
            int idConsulta;
            if (int.TryParse(textBox8.Text, out idConsulta))
            {
                string caminho = @"C:\IMC\IMC.json";

                if (File.Exists(caminho))
                {
                    string conteudoJson = File.ReadAllText(caminho);
                    List<Paciente> pacientes = JsonSerializer.Deserialize<List<Paciente>>(conteudoJson);

                    Paciente pacienteEncontrado = pacientes.FirstOrDefault(p => p.Id == idConsulta);

                    listBox1.Items.Clear(); // Limpa a lista antes de exibir o resultado da consulta

                    if (pacienteEncontrado != null)
                    {
                        // Exibe o paciente encontrado na listBox1
                        listBox1.Items.Add($"ID: {pacienteEncontrado.Id} - Nome: {pacienteEncontrado.Nome} - Peso: {pacienteEncontrado.Peso} kg - Altura: {pacienteEncontrado.Altura} m - IMC: {pacienteEncontrado.IMC:F2} - Classificação: {pacienteEncontrado.Classificacao}");
                    }
                    else
                    {
                        // Caso o ID não seja encontrado
                        listBox1.Items.Add("Paciente com o ID especificado não foi encontrado.");
                    }
                }
                else
                {
                    MessageBox.Show("Arquivo JSON não encontrado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Por favor, insira um ID válido.", "Entrada inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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

        private void button6_Click(object sender, EventArgs e)
        {
            Dictionary<string, int> contagemClassificacoes = ContarClassificacoes();
            GerarRelatorioHtml(contagemClassificacoes);

        }

        private void button7_Click(object sender, EventArgs e)
        {
            CarregarPacientes(); // Chama a função que carrega e exibe todos os pacientes na listBox1
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void textBox5_TextChanged(object sender, EventArgs e) { }
        private void textBox6_TextChanged(object sender, EventArgs e) { }
        private void textBox8_TextChanged(object sender, EventArgs e) { }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) { }

        private void label10_Click(object sender, EventArgs e)
        {

        }

    }
}