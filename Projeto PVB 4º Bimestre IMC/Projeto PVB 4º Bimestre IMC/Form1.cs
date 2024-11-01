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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO; //para gravação

namespace Projeto_PVB_4º_Bimestre_IMC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            string nome = textBox1.Text;
            textBox6.Text = nome;

            double peso = float.Parse(textBox2.Text);
            double altura = float.Parse(textBox3.Text);
            double IMC = peso / Math.Pow(altura, 2); //calculo do IMC
            string classificacao = "";
            textBox4.Text = IMC.ToString("00.00"); //impressão do imc formatado com 2 casas decimais

            if (IMC < 18.5) //Teste de condição para saber a classificação do nosso individuo
                classificacao = "Magreza";
            else if (IMC >= 18.5 && IMC <= 24.9)
                classificacao = "Saudável";
            else if (IMC >= 25.0 && IMC <= 29.9)
                classificacao = "Sobrepeso";
            else if (IMC >= 30.0 && IMC <= 34.9)
                classificacao = "Obesidade Grau I";
            else if (IMC >= 35.0 && IMC <= 39.9)
                classificacao = "Obesidade Grau II";
            else
                classificacao = "Obesidade Grau III";

            textBox5.Text = classificacao;

            SalvarDadosJson(nome, peso, altura, IMC, classificacao);
        }

        private void SalvarDadosJson(string nome, double peso, double altura, double IMC, string classificacao)
        {
            // Monta a estrutura JSON manualmente
            string json = "{\n" +
                          $"  \"nome\": \"{nome}\",\n" +
                          $"  \"peso\": {peso},\n" +
                          $"  \"altura\": {altura},\n" +
                          $"  \"IMC\": {IMC.ToString("00.00")},\n" +
                          $"  \"classificacao\": \"{classificacao}\"\n" +
                          "}";

            // Salva no arquivo
            string caminho = @"C:\IMC\IMC.json";
            File.WriteAllText(caminho, json);
        }


        private void button5_Click(object sender, EventArgs e)
        {
         
        }
    }
}
