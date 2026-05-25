import { Card, CardContent, CardHeader } from '../../../components/ui/Card';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts';

const publicationData = [
  { year: '2022', count: 24 },
  { year: '2023', count: 32 },
  { year: '2024', count: 41 },
  { year: '2025', count: 38 },
  { year: '2026', count: 21 },
];

const typeData = [
  { name: 'Journaux', value: 89 },
  { name: 'Conf. Int.', value: 45 },
  { name: 'Conf. Nat.', value: 22 },
];

const COLORS = ['#1a672f', '#1d3964', '#F59E0B'];

export default function SuperAdminStatistics() {
  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-bold text-navy dark:text-white">Statistiques</h1>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card>
          <CardHeader>
            <h3 className="text-lg font-bold text-navy dark:text-white">Publications par année</h3>
          </CardHeader>
          <CardContent>
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={publicationData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="year" />
                <YAxis />
                <Tooltip />
                <Bar dataKey="count" fill="#1a672f" />
              </BarChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <h3 className="text-lg font-bold text-navy dark:text-white">Publications par type</h3>
          </CardHeader>
          <CardContent>
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie data={typeData} cx="50%" cy="50%" labelLine={false} outerRadius={100} fill="#8884d8" dataKey="value" label={({name, percent}) => `${name} ${(percent * 100).toFixed(0)}%`}>
                  {typeData.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
