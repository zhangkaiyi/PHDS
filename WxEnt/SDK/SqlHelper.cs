using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Linq;

namespace Utility.DataBase
{

    /// <summary>
    /// Sql Server 数据库操作类
    /// </summary>
    public class SqlHelper : IDisposable
    {


        /// <summary>
        /// 默认构造函数
        /// </summary>
        public SqlHelper()
        {
        }
        /// <summary>
        /// 析构
        /// </summary>
        /// <summary>
        /// 资源释放委托实现
        /// </summary>
        private Action Destroy;

        /// <summary>
        /// SqlDataReader
        /// </summary>
        private SqlDataReader _SqlDataReader;

        /// <summary>
        /// 默认释放方法
        /// </summary>
        private void DefaultDispose()
        {
            if (_SqlDataReader != null)
            {
                _SqlDataReader.Close();
            }
            _SqlCommand.Dispose();
            _SqlConnection.Close();
            _SqlConnection.Dispose();
        }
        /// <summary>
        /// 启用事务操作的释放类
        /// </summary>
        private void TransactionDispose()
        {
            if (_SqlDataReader != null)
            {
                _SqlDataReader.Close();
                _SqlCommand.Dispose();
            }
        }

        /// <summary>
        /// 基础构造函数
        /// </summary>
        /// <param name="sqlConnection">数据库联接对象</param>
        public SqlHelper(SqlConnection sqlConnection)
        {
            _SqlConnection = sqlConnection;
            Destroy = DefaultDispose;
        }
        /// <summary>
        /// 通过事务构造对象
        /// </summary>
        /// <param name="Transaction"></param>
        public SqlHelper(SqlTransaction Transaction)
        {
            _Transaction = Transaction;
            Destroy = TransactionDispose;
        }
        /// <summary>
        /// 基础构造函数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        public SqlHelper(string connectionString)
        {
            ConnectionString = connectionString;
            Destroy = DefaultDispose;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="enableTransaction">是否开启事务处理</param>
        public SqlHelper(string connectionString, bool enableTransaction)
        {
            ConnectionString = connectionString;
            if (enableTransaction)
            {
                SqlConnection.Open();
                _Transaction = SqlConnection.BeginTransaction();
            }
            Destroy = DefaultDispose;
        }






        private System.Data.SqlClient.SqlTransaction _Transaction;
        /// <summary>
        /// 登记分布式事务
        /// </summary>
        public System.Data.SqlClient.SqlTransaction Transaction
        {
            get
            {
                return _Transaction;
            }
        }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return _SqlConnection.ConnectionString;
            }
            set
            {
                if (_SqlConnection != null)
                {
                    _SqlConnection.ConnectionString = value;
                }
                else
                {
                    _SqlConnection = new SqlConnection(value);
                }
            }
        }


        /// <summary>
        /// 数据库操作字符串
        /// </summary>
        public string CommandText
        {
            get
            {
                return _SqlCommand.CommandText;
            }
            set
            {
                if (_SqlCommand != null)
                {
                    _SqlCommand.CommandText = value;
                }
                else
                {
                    _SqlCommand = new SqlCommand(value);
                }
            }
        }


        private SqlConnection _SqlConnection = null;
        /// <summary>
        /// 数据库连接对象
        /// </summary>
        public SqlConnection SqlConnection
        {
            get
            {
                return _SqlConnection;
            }
            set
            {
                _SqlConnection = value;
            }
        }


        private SqlCommand _SqlCommand = new SqlCommand();
        /// <summary>
        /// 数据库操作对象
        /// </summary>
        public SqlCommand SqlCommand
        {
            get
            {
                return _SqlCommand;
            }
            set
            {
                _SqlCommand = value;
            }
        }


        /// <summary>
        /// SqlCommand 对象执行参数
        /// </summary>
        public SqlParameterCollection SqlParameter
        {
            get
            {
                return _SqlCommand.Parameters;
            }
            set
            {
                _SqlCommand.Parameters.Add(value);
            }
        }






        /// <summary>
        /// 执行 Sql 语句,并返回受影响的行数
        /// </summary>
        public int ExecuteNonQuery()
        {
            Connect();
            return _SqlCommand.ExecuteNonQuery();
        }
        /// <summary>
        /// 执行 Sql 语句,并返回受影响的行数
        /// </summary>
        /// <param name="CmdType">CommandText 属性</param>
        /// <returns></returns>
        public int ExecuteNonQuery(CommandType CmdType)
        {
            _SqlCommand.CommandType = CmdType;
            return ExecuteNonQuery();
        }
        /// <summary>
        /// 执行 Sql 语句,并返回受影响的行数
        /// </summary>
        /// <param name="ConnectionStr">数据库连接字符串</param>
        /// <param name="CommandText">执行文本</param>
        /// <param name="CommandType">执行类型</param>
        /// <param name="Params">参数</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string ConnectionStr, string CommandText, CommandType CommandType, IEnumerable<System.Data.SqlClient.SqlParameter> Params)
        {
            using (var Helper = new SqlHelper(ConnectionStr))
            {
                Helper.CommandText = CommandText;
                foreach (SqlParameter param in Params)
                {
                    if (param != null)
                    {
                        Helper.SqlParameter.Add(param);
                    }
                }
                return Helper.ExecuteNonQuery(CommandType);
            }
        }
        /// <summary>
        /// 执行 Sql 语句,并返回受影响的行数
        /// </summary>
        /// <param name="ConnectionStr">数据库连接字符串</param>
        /// <param name="CommandText">执行文本</param>
        /// <param name="CommandType">执行类型</param>
        /// <param name="Params">参数</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string ConnectionStr, string CommandText, CommandType CommandType, params System.Data.SqlClient.SqlParameter[] Params)
        {
            return ExecuteNonQuery(ConnectionStr, CommandText, CommandType, new List<SqlParameter>(Params));
        }
        /// <summary>
        /// 执行 Sql 语句,并返回受影响的行数
        /// </summary>
        /// <param name="ConnectionStr">数据库连接字符串</param>
        /// <param name="CommandText">执行文本</param>
        /// <param name="CommandType">执行类型</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string ConnectionStr, string CommandText, CommandType CommandType)
        {
            return ExecuteNonQuery(ConnectionStr, CommandText, CommandType, new List<SqlParameter>());
        }




        /// <summary>
        /// 返回执行结果的第一行第一列,忽略其他行或列
        /// </summary>
        public object ExecuteScalar()
        {
            Connect();
            return _SqlCommand.ExecuteScalar();
        }
        /// <summary>
        /// 返回执行结果的第一行第一列,忽略其他行或列
        /// </summary>
        /// <param name="CmdType">CommandText 属性</param>
        /// <returns></returns>
        public object ExecuteScalar(CommandType CmdType)
        {
            _SqlCommand.CommandType = CmdType;
            return ExecuteScalar();
        }

        /// <summary>
        /// 返回执行结果的第一行第一列,忽略其他行或列
        /// </summary>
        /// <param name="ConnectionStr">数据库连接字符串</param>
        /// <param name="CommandText">执行文本</param>
        /// <param name="CommandType">执行类型</param>
        /// <param name="Params">参数</param>
        /// <returns></returns>
        public static object ExecuteScalar(string ConnectionStr, string CommandText, CommandType CommandType, IEnumerable<System.Data.SqlClient.SqlParameter> Params)
        {
            using (var Helper = new SqlHelper(ConnectionStr))
            {
                Helper.CommandText = CommandText;
                foreach (SqlParameter param in Params)
                {
                    if (param != null)
                    {
                        Helper.SqlParameter.Add(param);
                    }
                }
                return Helper.ExecuteScalar(CommandType);
            }
        }

        /// <summary>
        /// 返回执行结果的第一行第一列,忽略其他行或列
        /// </summary>
        /// <param name="ConnectionStr">数据库连接字符串</param>
        /// <param name="CommandText">执行文本</param>
        /// <param name="CommandType">执行类型</param>
        /// <param name="Params">参数</param>
        /// <returns></returns>
        public static object ExecuteScalar(string ConnectionStr, string CommandText, CommandType CommandType, params System.Data.SqlClient.SqlParameter[] Params)
        {
            return ExecuteScalar(ConnectionStr, CommandText, CommandType, new List<SqlParameter>(Params));
        }
        /// <summary>
        /// 返回执行结果的第一行第一列,忽略其他行或列
        /// </summary>
        /// <param name="ConnectionStr">数据库连接字符串</param>
        /// <param name="CommandText">执行文本</param>
        /// <param name="CommandType">执行类型</param>
        /// <returns></returns>
        public static object ExecuteScalar(string ConnectionStr, string CommandText, CommandType CommandType)
        {
            return ExecuteScalar(ConnectionStr, CommandText, CommandType, new List<SqlParameter>());
        }





        /// <summary>
        /// 返回 SqlDataReader
        /// </summary>
        /// <returns></returns>
        public SqlDataReader ExecuteReader()
        {
            Connect();
            _SqlDataReader = _SqlCommand.ExecuteReader();
            return _SqlDataReader;
        }

        /// <summary>
        /// 返回 SqlDataReader
        /// </summary>
        /// <param name="CmdType">CommandText 属性</param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader(CommandType CmdType)
        {
            _SqlCommand.CommandType = CmdType;
            return ExecuteReader();
        }
        /// <summary>
        /// 返回 SqlDataReader
        /// </summary>
        /// <param name="Behavior">查询结果或查询对象对数据库的影响</param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader(System.Data.CommandBehavior Behavior)
        {
            Connect();
            _SqlDataReader = _SqlCommand.ExecuteReader(Behavior);
            return _SqlDataReader;
        }
        /// <summary>
        /// 返回 SqlDataReader
        /// </summary>
        /// <param name="CmdType">CommandText 属性</param>
        /// <param name="Behavior">查询结果或查询对象对数据库的影响</param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader(CommandType CmdType, System.Data.CommandBehavior Behavior)
        {
            _SqlCommand.CommandType = CmdType;
            return ExecuteReader(Behavior);
        }

        /// <summary>
        /// 返回 SqlDataReader
        /// </summary>
        /// <param name="ConnectionStr">数据库连接字符串</param>
        /// <param name="CommandText">执行文本</param>
        /// <param name="CommandType">执行类型</param>
        /// <param name="Params">参数</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string ConnectionStr, string CommandText, CommandType CommandType, IEnumerable<System.Data.SqlClient.SqlParameter> Params)
        {
            var Helper = new SqlHelper(ConnectionStr);
            Helper.CommandText = CommandText;
            foreach (SqlParameter param in Params)
            {
                if (param != null)
                {
                    Helper.SqlParameter.Add(param);
                }
            }
            return Helper.ExecuteReader(CommandType, CommandBehavior.CloseConnection);
        }
        /// <summary>
        /// 返回 SqlDataReader
        /// </summary>
        /// <param name="ConnectionStr">数据库连接字符串</param>
        /// <param name="CommandText">执行文本</param>
        /// <param name="CommandType">执行类型</param>
        /// <param name="Params">参数</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string ConnectionStr, string CommandText, CommandType CommandType, params System.Data.SqlClient.SqlParameter[] Params)
        {
            return ExecuteReader(ConnectionStr, CommandText, CommandType, new List<SqlParameter>(Params));
        }
        /// <summary>
        /// 返回 SqlDataReader
        /// </summary>
        /// <param name="ConnectionStr">数据库连接字符串</param>
        /// <param name="CommandText">执行文本</param>
        /// <param name="CommandType">执行类型</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string ConnectionStr, string CommandText, CommandType CommandType)
        {
            return ExecuteReader(ConnectionStr, CommandText, CommandType, new List<SqlParameter>());
        }





        /// <summary>
        /// 返回 DataSet
        /// </summary>
        /// <returns></returns>
        public DataTable GetDataTable()
        {
            Connect();
            var _Adapter = new SqlDataAdapter(_SqlCommand);
            var _DataTable = new DataTable();
            _Adapter.Fill(_DataTable);
            return _DataTable;
        }
        /// <summary>
        /// 返回 DataSet
        /// </summary>
        /// <returns></returns>
        public DataTable GetDataTable(CommandType CmdType)
        {
            _SqlCommand.CommandType = CmdType;
            return GetDataTable();
        }
        /// <summary>
        /// 返回 DataSet
        /// </summary>
        /// <param name="ConnectionStr">数据库连接字符串</param>
        /// <param name="CommandText">执行命令</param>
        /// <param name="CommandType">执行类型</param>
        /// <param name="Params">参数</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string ConnectionStr, string CommandText, CommandType CommandType, IEnumerable<System.Data.SqlClient.SqlParameter> Params)
        {
            using (var Helper = new SqlHelper(ConnectionStr))
            {
                Helper.CommandText = CommandText;
                foreach (SqlParameter param in Params)
                {
                    Helper.SqlParameter.Add(param);
                }
                return Helper.GetDataTable(CommandType);
            }
        }

        /// <summary>
        /// 返回 DataSet
        /// </summary>
        /// <param name="ConnectionStr">数据库连接字符串</param>
        /// <param name="CommandText">执行命令</param>
        /// <param name="CommandType">执行类型</param>
        /// <param name="Params">参数</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string ConnectionStr, string CommandText, CommandType CommandType, params System.Data.SqlClient.SqlParameter[] Params)
        {
            return GetDataTable(ConnectionStr, CommandText, CommandType, new List<SqlParameter>(Params));
        }



        /// <summary>
        /// 清除数据库操作对象的参数及数据库操作字符串
        /// </summary>
        public void ClearParameters()
        {
            _SqlCommand.Parameters.Clear();
            _SqlCommand.CommandText = string.Empty;
        }

        /// <summary>
        /// 创建数据库连接
        /// </summary>
        private void Connect()
        {
            if (_Transaction != null)
            {
                _SqlCommand.Connection = _Transaction.Connection;
                _SqlCommand.Transaction = _Transaction;
            }
            else
            {
                _SqlCommand.Connection = _SqlConnection;

                if (_SqlConnection.State == ConnectionState.Broken)
                {
                    _SqlConnection.Close();
                }
                if (_SqlConnection.State != ConnectionState.Open)
                {
                    _SqlConnection.Open();
                }
            }
        }

        /// <summary>
        /// 释放 SqlHelper 对象所释放的资源,继承自 IDisposable 接口
        /// </summary>
        public void Dispose()
        {
            if (Destroy != null)
            {
                Destroy();
            }
        }
    }
    /// <summary>
    /// SqlHelper 辅助类
    /// </summary>
    public class SqlHelperAssist
    {
        /// <summary>
        /// 表缓存
        /// </summary>
        private static Dictionary<string, string> tableCache = new Dictionary<string, string>();
        /// <summary>
        /// 字段缓存
        /// </summary>
        private static Dictionary<string, Dictionary<string, DataFieldAttribute>> fieldCache = new Dictionary<string, Dictionary<string, DataFieldAttribute>>();
        /// <summary>
        /// 通过指定的参数与值获得所需要的字段值
        /// </summary>
        /// <typeparam name="TModel">需要返回的类型</typeparam>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="conditionField">传递的参数（是模型中的字段，不是数据库中的字段）</param>
        /// <param name="conditionValue">参数值，若该值为对象，则该对象必须与 conditionField 所指向的类型对象相同。程序则自动取该对象的标识列作为值。</param>
        /// <param name="getFields">需要填充的字段（是模型中的字段，不是数据库中的字段）</param>
        /// <returns></returns>
        public static TModel Get<TModel>(string connectionString, string conditionField,
            object conditionValue, params string[] getFields)
            where TModel: new()
        {
            var _t = typeof(TModel);

            if (!CacheMapping(_t))
            {
                throw new Exception("类型：" + _t.FullName + " 未标记 DataField 特性。");
            }

            var _temp = new Dictionary<string, DataFieldAttribute>();

            if (getFields.Length == 0)
            {
                _temp = fieldCache[_t.FullName];
            }
            else
            {
                foreach (string _field in getFields)
                {
                    _temp.Add(_field, fieldCache[_t.FullName][_field]);
                }
            }

            var commandText = "Select Top 1 {0} From [{1}] Where [{2}]=@_{2}";


            var _tempField = fieldCache[_t.FullName][conditionField];

            var Sb = new StringBuilder();

            foreach (KeyValuePair<string, DataFieldAttribute> _fa in _temp)
            {
                Sb.Append(string.Format("[{0}],", _fa.Value.Name));
            }
            commandText = string.Format(commandText, Sb.ToString().TrimEnd(','), tableCache[_t.FullName], _tempField.Name);





            var conditionProperty = _t.GetProperty(conditionField);

            var param = new SqlParameter();
            param.ParameterName = "@_" + _tempField.Name;


            if (conditionProperty.PropertyType.IsSealed)
            {
                param.SqlDbType = _tempField.Type;
                if (_tempField.Length != -1)
                {
                    param.Size = _tempField.Length;
                }
                param.Value = conditionValue;
            }
            else
            {
                if (conditionProperty.PropertyType.IsClass)
                {
                    if (!CacheMapping(conditionProperty.PropertyType))
                    {
                        throw new Exception("类型：" + conditionProperty.PropertyType.FullName + " 未标记 DataField 特性。");
                    }
                    var _tempDataField = fieldCache[conditionProperty.PropertyType.FullName].FirstOrDefault(i => i.Value.IsIdentity == true);
                    if (_tempDataField.Value != null)
                    {
                        param.SqlDbType = _tempDataField.Value.Type;
                        if (_tempDataField.Value.Length != -1)
                        {
                            param.Size = _tempDataField.Value.Length;
                        }
                        var _cValueType = conditionValue.GetType();

                        if (_cValueType.IsSealed)
                        {
                            param.Value = conditionValue;
                        }
                        else
                        {
                            if (_cValueType.IsClass)
                            {
                                param.Value = conditionProperty.PropertyType.GetProperty(_tempDataField.Key).GetValue(conditionValue, null);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("未找到：" + conditionProperty.PropertyType.FullName + " 所需要的值。\r\n是否未定义 IsIdentification 字段？");
                    }
                }
                else
                {
                    throw new Exception("未知的类型：" + conditionProperty.PropertyType.FullName);
                }
            }
            using (var Sdr = SqlHelper.ExecuteReader(connectionString, commandText,
                CommandType.Text, param))
            {
                var result = new TModel();

                while (Sdr.Read())
                {
                    foreach (KeyValuePair<string, DataFieldAttribute> dataField in _temp)
                    {
                        var property = _t.GetProperty(dataField.Key);


                        if (property.PropertyType.IsSealed)
                        {
                            property.SetValue(result, Sdr[dataField.Value.Name], null);
                        }



                        else
                        {
                            if (property.PropertyType.IsClass)
                            {
                                property.PropertyType.GetType();
                                if (!CacheMapping(property.PropertyType))
                                {
                                    throw new Exception("类型：" + property.PropertyType.FullName + " 未标记 DataField 特性。");
                                }
                                var _tempDataField = fieldCache[property.PropertyType.FullName].FirstOrDefault(i => i.Value.IsIdentity == true);
                                if (_tempDataField.Value != null)
                                {
                                    var _obj = property.PropertyType.Assembly.CreateInstance(property.PropertyType.FullName);
                                    property.PropertyType.GetProperty(_tempDataField.Key).SetValue(_obj, Sdr[dataField.Value.Name], null);
                                    property.SetValue(result, _obj, null);
                                }
                                else
                                {
                                    throw new Exception("未找到：" + property.PropertyType.FullName + " 所需要的值。\r\n是否未定义 IsIdentification 字段？");
                                }
                            }
                            else
                            {
                                throw new Exception("未知的类型：" + property.PropertyType.FullName);
                            }
                        }
                    }

                    return result;
                }

                return default(TModel);
            }
        }

        /// <summary>
        /// 按条件获取指定条件的条数
        /// </summary>
        /// <typeparam name="TModel">对象类型</typeparam>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="conditionField">条件字段名（模型中的字段，不是数据库中的字段。）</param>
        /// <param name="conditionValue">条件值</param>
        /// <returns></returns>
        public static int GetCount<TModel>(string connectionString, string conditionField = null,
            object conditionValue = null)
            where TModel: new()
        {
            var _t = typeof(TModel);

            if (!CacheMapping(_t))
            {
                throw new Exception("类型：" + _t.FullName + " 未标记 DataField 特性。");
            }
            var commandText = string.Format("Select Count(*) From [{0}]", tableCache[_t.FullName]);

            if (conditionField == null)
            {
                return (int)SqlHelper.ExecuteScalar(connectionString, commandText, CommandType.Text);
            }

            var _tempField = fieldCache[_t.FullName][conditionField];
            commandText += string.Format(" Where [{0}]=@_{0}", _tempField.Name);

            var conditionParam = new SqlParameter();
            conditionParam.ParameterName = "@_" + _tempField.Name;
            conditionParam.SqlDbType = _tempField.Type;
            if (_tempField.Length != -1)
            {
                conditionParam.Size = _tempField.Length;
            }
            var _cValueType = conditionValue.GetType();

            if (_cValueType.IsSealed)
            {
                conditionParam.Value = conditionValue;
            }
            else
            {
                if (_cValueType.IsClass)
                {
                    conditionParam.Value = _t.GetProperty(conditionField).GetValue(conditionValue, null);
                }
            }
            return (int)SqlHelper.ExecuteScalar(connectionString, commandText, CommandType.Text, conditionParam);
        }

        public static List<TModel> GetList<TModel>(string connectionString, string conditionField = null, object conditionValue = null,
        params string[] getFields)
            where TModel: new()
        {
            var _t = typeof(TModel);

            if (!CacheMapping(_t))
            {
                throw new Exception("类型：" + _t.FullName + " 未标记 DataField 特性。");
            }

            var _temp = new Dictionary<string, DataFieldAttribute>();

            if (getFields.Length == 0)
            {
                _temp = fieldCache[_t.FullName];
            }
            else
            {
                foreach (string _field in getFields)
                {
                    _temp.Add(_field, fieldCache[_t.FullName][_field]);
                }
            }
            var commandText = "Select {0} From [{1}]";

            var Sb = new StringBuilder();

            foreach (KeyValuePair<string, DataFieldAttribute> _fa in _temp)
            {
                Sb.Append(string.Format("[{0}],", _fa.Value.Name));
            }
            commandText = string.Format(commandText, Sb.ToString().TrimEnd(','), tableCache[_t.FullName]);

            SqlParameter conditionParam = null;



            if (conditionField != null)
            {
                var _tempField = fieldCache[_t.FullName][conditionField];

                var conditionProperty = _t.GetProperty(conditionField);

                commandText += string.Format(" Where [{0}]=@_{0}", _tempField.Name);
                conditionParam = new SqlParameter();
                conditionParam.ParameterName = "@_" + _tempField.Name;

                if (conditionProperty.PropertyType.IsSealed)
                {
                    conditionParam.SqlDbType = _tempField.Type;
                    if (_tempField.Length != -1)
                    {
                        conditionParam.Size = _tempField.Length;
                    }
                    conditionParam.Value = conditionValue;
                }
                else
                {
                    if (conditionProperty.PropertyType.IsClass)
                    {
                        if (!CacheMapping(conditionProperty.PropertyType))
                        {
                            throw new Exception("类型：" + conditionProperty.PropertyType.FullName + " 未标记 DataField 特性。");
                        }
                        var _tempDataField = fieldCache[conditionProperty.PropertyType.FullName].FirstOrDefault(i => i.Value.IsIdentity == true);
                        if (_tempDataField.Value != null)
                        {
                            conditionParam.SqlDbType = _tempDataField.Value.Type;
                            if (_tempDataField.Value.Length != -1)
                            {
                                conditionParam.Size = _tempDataField.Value.Length;
                            }
                            var _cValueType = conditionValue.GetType();

                            if (_cValueType.IsSealed)
                            {
                                conditionParam.Value = conditionValue;
                            }
                            else
                            {
                                if (_cValueType.IsClass)
                                {
                                    conditionParam.Value = conditionProperty.PropertyType.GetProperty(_tempDataField.Key).GetValue(conditionValue, null);
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("未找到：" + conditionProperty.PropertyType.FullName + " 所需要的值。\r\n是否未定义 IsIdentification 字段？");
                        }
                    }
                    else
                    {
                        throw new Exception("未知的类型：" + conditionProperty.PropertyType.FullName);
                    }
                }
            }

            using (var Sdr = SqlHelper.ExecuteReader(connectionString, commandText, CommandType.Text, conditionParam))
            {
                var modelList = new List<TModel>();

                while (Sdr.Read())
                {
                    var model = new TModel();

                    foreach (KeyValuePair<string, DataFieldAttribute> dataField in _temp)
                    {
                        var property = _t.GetProperty(dataField.Key);


                        if (property.PropertyType.IsSealed)
                        {
                            property.SetValue(model, Sdr[dataField.Value.Name], null);
                        }
                        else
                        {
                            if (property.PropertyType.IsClass)
                            {
                                property.PropertyType.GetType();
                                if (!CacheMapping(property.PropertyType))
                                {
                                    throw new Exception("类型：" + property.PropertyType.FullName + " 未标记 DataField 特性。");
                                }
                                var _tempDataField = fieldCache[property.PropertyType.FullName].FirstOrDefault(i => i.Value.IsIdentity == true);
                                if (_tempDataField.Value != null)
                                {
                                    var _obj = property.PropertyType.Assembly.CreateInstance(property.PropertyType.FullName);
                                    property.PropertyType.GetProperty(_tempDataField.Key).SetValue(_obj, Sdr[dataField.Value.Name], null);
                                    property.SetValue(model, _obj, null);
                                }
                                else
                                {
                                    throw new Exception("未找到：" + property.PropertyType.FullName + " 所需要的值。\r\n是否未定义 IsIdentification 字段？");
                                }
                            }
                            else
                            {
                                throw new Exception("未知的类型：" + property.PropertyType.FullName);
                            }
                        }
                    }

                    modelList.Add(model);
                }

                return modelList;
            }
        }
        /// <summary>
        /// 将指定类型的数据模型数据插入到数据库中，标识列不进行操作。
        /// </summary>
        /// <typeparam name="TModel">要插入的模型类型</typeparam>
        /// <typeparam name="TResult">要返回的模型类型，一般为 int 类型。该值是插入到数据库后返回的标识列。</typeparam>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dataModel">包含要插入数据的数据模型</param>
        /// <param name="fields">
        /// 设定进行插入操作的字段，不填写则默认插入所有设置了特性的字段。
        /// 此处字段为模型内字段，不是数据库字段。
        /// </param>
        /// <returns>返回值类型（TResult）的主键值，若表中未设置主键，则返回 默认值(int为0,string为null)。</returns>
        public static TResult Insert<TModel, TResult>(string connectionString, TModel dataModel,
        params string[] fields)
            where TModel: new()
            {
                var _t = typeof(TModel);

                if (!CacheMapping(_t))
                {
                    throw new Exception("类型：" + _t.FullName + " 未标记 DataField 特性。");
                }

                var commandText = "Insert Into [{0}] ({1}) Values({2});";

                var _temp = new Dictionary<string, DataFieldAttribute>();


                if (fields.Length != 0)
                {
                    foreach (string field in fields)
                    {
                        if (fieldCache[_t.FullName].ContainsKey(field))
                        {
                            _temp.Add(field, fieldCache[_t.FullName][field]);
                        }
                    }
                }
                else
                {
                    _temp = fieldCache[_t.FullName];
                }
                var Sb = new StringBuilder();
                var parms = new List<SqlParameter>();


                foreach (KeyValuePair<string, DataFieldAttribute> dataField in _temp)
                {
                    if (dataField.Value.IsIdentity)
                    {
                        continue;
                    }
                    Sb.Append(string.Format("[{0}],", dataField.Value.Name));

                    var property = _t.GetProperty(dataField.Key);

                    var parm = new SqlParameter();
                    parm.ParameterName = "@" + dataField.Value.Name;


                    if (property.PropertyType.IsSealed)
                    {
                        parm.SqlDbType = dataField.Value.Type;
                        if (dataField.Value.Length != -1)
                        {
                            parm.Size = dataField.Value.Length;
                        }
                        parm.Value = property.GetValue(dataModel, null);
                    }
                    else
                    {
                        if (property.PropertyType.IsClass)
                        {
                            property.PropertyType.GetType();
                            if (!CacheMapping(property.PropertyType))
                            {
                                throw new Exception("类型：" + property.PropertyType.FullName + " 未标记 DataField 特性。");
                            }
                            var _tempDataField = fieldCache[property.PropertyType.FullName].FirstOrDefault(i => i.Value.IsIdentity == true);
                            if (_tempDataField.Value != null)
                            {
                                var _obj = property.GetValue(dataModel, null);
                                parm.SqlDbType = _tempDataField.Value.Type;
                                if (_tempDataField.Value.Length != -1)
                                {
                                    parm.Size = _tempDataField.Value.Length;
                                }
                                parm.Value = property.PropertyType.GetProperty(_tempDataField.Key).GetValue(_obj, null);
                            }
                            else
                            {
                                throw new Exception("未找到：" + property.PropertyType.FullName + " 所需要的值。\r\n是否未定义 IsIdentification 字段？");
                            }
                        }
                        else
                        {
                            throw new Exception("未知的类型：" + property.PropertyType.FullName);
                        }
                    }
                    parms.Add(parm);
                }

                var _tmp = Sb.ToString().TrimEnd(',');
                commandText = string.Format(commandText, tableCache[_t.FullName], _tmp, _tmp.Replace('[', '@').Replace("]", string.Empty));



                if (typeof(TResult) == typeof(bool))
                {
                    var obj = SqlHelper.ExecuteNonQuery(connectionString, commandText,
                    CommandType.Text, parms) > 0;

                    return (TResult)Convert.ChangeType(obj, typeof(TResult));
                }
                else
                {
                    var obj = SqlHelper.ExecuteScalar(connectionString, commandText + "Select @@identity",
                    CommandType.Text, parms);


                    if (obj.Equals(System.DBNull.Value))
                    {
                        return default(TResult);
                    }
                    return (TResult)Convert.ChangeType(obj, typeof(TResult));
                }
            }
            /// <summary>
            /// 按指定字段条件删除数据
            /// </summary>
            /// <typeparam name="TModel">模型类型</typeparam>
            /// <param name="connectionString">数据库连接字符串</param>
            /// <param name="conditionField">要删除的条件字段（这里要的字段是模型中的字段，并非数据库中的字段）</param>
            /// <param name="conditionValue">字段值。</param>
            /// <returns>返回影响的行数。</returns>
            public static int Delete<TModel>(string connectionString, string conditionField, object conditionValue)
            {
                var _t = typeof(TModel);

                if (!CacheMapping(_t))
                {
                    throw new Exception("类型：" + _t.FullName + " 未标记 DataField 特性。");
                }
                var dataField = fieldCache[_t.FullName][conditionField];

                var commandText = string.Format("Delete From [{0}] Where [{1}]=@_{1}", tableCache[_t.FullName], dataField.Name);

                var conditionProperty = _t.GetProperty(conditionField);

                var conditionParm = new SqlParameter();
                conditionParm.ParameterName = "@_" + dataField.Name;

                if (conditionProperty.PropertyType.IsSealed)
                {
                    conditionParm.SqlDbType = dataField.Type;
                    if (dataField.Length != -1)
                    {
                        conditionParm.Size = dataField.Length;
                    }
                    conditionParm.Value = conditionValue;
                }
                else
                {
                    if (conditionProperty.PropertyType.IsClass)
                    {
                        if (!CacheMapping(conditionProperty.PropertyType))
                        {
                            throw new Exception("类型：" + conditionProperty.PropertyType.FullName + " 未标记 DataField 特性。");
                        }
                        var _tempDataField = fieldCache[conditionProperty.PropertyType.FullName].FirstOrDefault(i => i.Value.IsIdentity == true);
                        if (_tempDataField.Value != null)
                        {
                            conditionParm.SqlDbType = _tempDataField.Value.Type;
                            if (_tempDataField.Value.Length != -1)
                            {
                                conditionParm.Size = _tempDataField.Value.Length;
                            }
                            var _cValueType = conditionValue.GetType();

                            if (_cValueType.IsSealed)
                            {
                                conditionParm.Value = conditionValue;
                            }
                            else
                            {
                                if (_cValueType.IsClass)
                                {
                                    conditionParm.Value = conditionProperty.PropertyType.GetProperty(_tempDataField.Key).GetValue(conditionValue, null);
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("未找到：" + conditionProperty.PropertyType.FullName + " 所需要的值。\r\n是否未定义 IsIdentification 字段？");
                        }
                    }
                    else
                    {
                        throw new Exception("未知的类型：" + conditionProperty.PropertyType.FullName);
                    }
                }
                return SqlHelper.ExecuteNonQuery(connectionString, commandText, CommandType.Text, conditionParm);
            }
            /// <summary>
            /// 按指定条件更新数据
            /// </summary>
            /// <typeparam name="TModel">模型类型</typeparam>
            /// <param name="connectionString">数据库连接字符串</param>
            /// <param name="dataModel">数据模型</param>
            /// <param name="conditionField">条件字段名（此字段名为模型中的名称，并非是数据库中字段名），如果此参数为 null 则自动以标识字段作为字段名。</param>
            /// <param name="conditionValue">条件字段的制定值，若不指定或指定为 null 则自动取以上指定字段的数据模型中的值。</param>
            /// <param name="fields">需要更新的字段，不填写则为所有字段。</param>
            /// <returns>响应的行数</returns>
            public static int Update<TModel>(string connectionString, TModel dataModel, string conditionField = null,
            object conditionValue = null, params string[] fields)
                where TModel: new()
            {
                var _t = typeof(TModel);

                if (!CacheMapping(_t))
                {
                    throw new Exception("类型：" + _t.FullName + " 未标记 DataField 特性。");
                }
                var _temp = new Dictionary<string, DataFieldAttribute>();


                if (fields.Length != 0)
                {
                    foreach (string field in fields)
                    {
                        if (fieldCache[_t.FullName].ContainsKey(field))
                        {
                            _temp.Add(field, fieldCache[_t.FullName][field]);
                        }
                    }
                }
                else
                {
                    _temp = fieldCache[_t.FullName];
                }

                var Sb = new StringBuilder();
                var parms = new List<SqlParameter>();

                foreach (KeyValuePair<string, DataFieldAttribute> dataField in _temp)
                {
                    if (dataField.Value.IsIdentity)
                    {
                        continue;
                    }
                    Sb.Append(string.Format("[{0}]=@{0},", dataField.Value.Name));

                    var property = _t.GetProperty(dataField.Key);

                    var parm = new SqlParameter();
                    parm.ParameterName = "@" + dataField.Value.Name;

                    if (property.PropertyType.IsSealed)
                    {
                        parm.SqlDbType = dataField.Value.Type;
                        if (dataField.Value.Length != -1)
                        {
                            parm.Size = dataField.Value.Length;
                        }
                        parm.Value = property.GetValue(dataModel, null);
                    }
                    else
                    {
                        if (property.PropertyType.IsClass)
                        {
                            if (!CacheMapping(property.PropertyType))
                            {
                                throw new Exception("类型：" + property.PropertyType.FullName + " 未标记 DataField 特性。");
                            }
                            var _tempDataField = fieldCache[property.PropertyType.FullName].FirstOrDefault(i => i.Value.IsIdentity == true);
                            if (_tempDataField.Value != null)
                            {
                                var _obj = property.GetValue(dataModel, null);
                                parm.SqlDbType = _tempDataField.Value.Type;
                                if (_tempDataField.Value.Length != -1)
                                {
                                    parm.Size = _tempDataField.Value.Length;
                                }
                                parm.Value = property.PropertyType.GetProperty(_tempDataField.Key).GetValue(_obj, null);
                            }
                            else
                            {
                                throw new Exception("未找到：" + property.PropertyType.FullName + " 所需要的值。\r\n是否未定义 IsIdentification 字段？");
                            }
                        }
                        else
                        {
                            throw new Exception("未知的类型：" + property.PropertyType.FullName);
                        }
                    }
                    parms.Add(parm);
                }






                DataFieldAttribute conditionDataField = null;

                if (conditionField != null)
                {
                    conditionDataField = fieldCache[_t.FullName][conditionField];
                }
                else
                {
                    var _tempDataField = fieldCache[_t.FullName].FirstOrDefault(i => i.Value.IsIdentity == true);
                    conditionField = _tempDataField.Key;
                    conditionDataField = _tempDataField.Value;
                }

                var commandText = string.Format("Update [{0}] Set {1} Where [{2}]=@_{2}",
                tableCache[_t.FullName], Sb.ToString().TrimEnd(','), conditionDataField.Name);

                var conditionProperty = _t.GetProperty(conditionField);

                var conditionParm = new SqlParameter();
                conditionParm.ParameterName = "@_" + conditionDataField.Name;

                if (conditionProperty.PropertyType.IsSealed)
                {
                    conditionParm.SqlDbType = conditionDataField.Type;
                    if (conditionDataField.Length != -1)
                    {
                        conditionParm.Size = conditionDataField.Length;
                    }
                    if (conditionValue == null)
                    {
                        conditionParm.Value = conditionProperty.GetValue(dataModel, null);
                    }
                    else
                    {
                        conditionParm.Value = conditionValue;
                    }
                }
                else
                {
                    if (conditionProperty.PropertyType.IsClass)
                    {
                        if (!CacheMapping(conditionProperty.PropertyType))
                        {
                            throw new Exception("类型：" + conditionProperty.PropertyType.FullName + " 未标记 DataField 特性。");
                        }
                        var _tempDataField = fieldCache[conditionProperty.PropertyType.FullName].FirstOrDefault(i => i.Value.IsIdentity == true);
                        if (_tempDataField.Value != null)
                        {
                            var _obj = conditionProperty.GetValue(dataModel, null);
                            conditionParm.SqlDbType = _tempDataField.Value.Type;
                            if (_tempDataField.Value.Length != -1)
                            {
                                conditionParm.Size = _tempDataField.Value.Length;
                            }
                            if (conditionValue == null)
                            {
                                conditionParm.Value = conditionProperty.PropertyType.GetProperty(_tempDataField.Key).GetValue(_obj, null);
                            }
                            else
                            {
                                var _cValueType = conditionValue.GetType();

                                if (_cValueType.IsSealed)
                                {
                                    conditionParm.Value = conditionValue;
                                }
                                else
                                {
                                    if (_cValueType.IsClass)
                                    {
                                        conditionParm.Value = conditionProperty.PropertyType.GetProperty(_tempDataField.Key).GetValue(conditionValue, null);
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("未找到：" + conditionProperty.PropertyType.FullName + " 所需要的值。\r\n是否未定义 IsIdentification 字段？");
                        }
                    }
                    else
                    {
                        throw new Exception("未知的类型：" + conditionProperty.PropertyType.FullName);
                    }
                }
                parms.Add(conditionParm);



                return SqlHelper.ExecuteNonQuery(connectionString, commandText, CommandType.Text, parms);
            }


            /// <summary>
            /// 缓存映射关系
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="_t"></param>
            /// <returns></returns>
            private static bool CacheMapping(Type _t)
            {
                lock (tableCache)
                {
                    if (!tableCache.ContainsKey(_t.FullName))
                    {
                        var dataFields = (DataFieldAttribute[])_t.GetCustomAttributes(typeof(DataFieldAttribute), false);
                        if (dataFields.Length == 0)
                        {
                            return false;
                        }
                        tableCache.Add(_t.FullName, dataFields[0].Name);
                    }
                }



                lock (fieldCache)
                {
                    if (!fieldCache.ContainsKey(_t.FullName))
                    {
                        var propertysInfo = _t.GetProperties();

                        if (propertysInfo.Length == 0)
                        {
                            return false;
                        }
                        var _tempCache = new Dictionary<string, DataFieldAttribute>();

                        foreach (PropertyInfo propInfo in propertysInfo)
                        {
                            var _propertyDataFields = (DataFieldAttribute[])propInfo.GetCustomAttributes(typeof(DataFieldAttribute), false);
                            if (_propertyDataFields.Length == 0)
                            {
                                continue;
                            }
                            _tempCache.Add(propInfo.Name, _propertyDataFields[0]);
                        }

                        fieldCache.Add(_t.FullName, _tempCache);
                    }
                }


                return true;
            }
        }


        /// <summary>
        /// 字段映射
        /// </summary>
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
        public class DataFieldAttribute : System.Attribute
        {
            private string _Name = string.Empty;
            /// <summary>
            /// 字段名
            /// </summary>
            public string Name
            {
                get
                {
                    return _Name;
                }
                set
                {
                    _Name = value;
                }
            }
            private System.Data.SqlDbType _Type = SqlDbType.Int;
            /// <summary>
            /// 字段类型
            /// </summary>
            public System.Data.SqlDbType Type
            {
                get
                {
                    return _Type;
                }
                set
                {
                    _Type = value;
                }
            }
            private int _Length = -1;
            /// <summary>
            /// 字段长度
            /// </summary>
            public int Length
            {
                get
                {
                    return _Length;
                }
                set
                {
                    _Length = value;
                }
            }

            private bool _IsIdentity = false;
            /// <summary>
            /// 是否为标识列
            /// </summary>
            public bool IsIdentity
            {
                get
                {
                    return _IsIdentity;
                }
                set
                {
                    _IsIdentity = value;
                }
            }
            /// <summary>
            /// 设置字段属性
            /// </summary>
            /// <param name="Name">字段名</param>
            /// <param name="Type">字段类型</param>
            /// <param name="Length">字段长度（Int,DateTime,Date,Bit,Text 等长度为 -1 默认可以不填写）</param>
            /// <param name="IsIdentity">是否为标识字段（自动生成字段，如编号，标识字段在插入新数据时不进行插入操作）</param>
            public DataFieldAttribute(string Name, System.Data.SqlDbType Type = SqlDbType.Int, int Length = -1, bool IsIdentity = false)
            {
                _Name = Name;
                _Type = Type;
                _Length = Length;
                _IsIdentity = IsIdentity;
            }
        }
    }
